using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ResumeBuilder.Application;
using ResumeBuilder.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using MongoDB.Driver;
using ResumeBuilder.Infrastructure.Repositories.Users.Models;
using ResumeBuilder.Infrastructure.Repositories.Users;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder;
#if DEBUG
        builder = GetConfiguredBuilder(args);
#else
        builder = WebApplication.CreateBuilder(args);
#endif
        builder.Services.AddControllers();
        var config = builder.Configuration;
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        ConfigureMongoDb(builder.Services, config);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();
        //}

        // Swagger UI is currently enabled for non-dev environment for ease of testing
        app.UseSwagger();

        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static WebApplicationBuilder GetConfiguredBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile($"appsettings.Development.json");
        return builder;
    }

    private static IMongoDatabase CreateMongoDatabase(string connectionString, IConfiguration config)
    {
        var client = new MongoClient(connectionString);
        return client.GetDatabase(config["CareerVentureDatabaseSettings:DatabaseName"]);
    }

    private static void ConfigureMongoDb(IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("AZURE_MONGODB_CONNECTIONSTRING");
        if (connectionString == null)
            throw new MongoConfigurationException("Database settings not found");

        try
        {
            var db = CreateMongoDatabase(connectionString, config);
            AddMongoDbRepository<UserRepository, UserInfra>(config["CareerVentureDatabaseSettings:UsersCollectionName"]!);

            void AddMongoDbRepository<TRepository, TModel>(string collectionName)
            {
                services.AddSingleton(db.GetCollection<TModel>(collectionName));
                services.AddSingleton(typeof(TRepository));
            }
        }
        catch (ArgumentNullException ex)
        {
            throw new MongoConfigurationException("Database settings are invalid", ex);
        }
        catch (Exception ex)
        {
            throw new MongoConfigurationException("Error configuring MongoDB", ex);
        }
    }
}