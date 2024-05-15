using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ResumeBuilder.Application;
using ResumeBuilder.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MongoDB.Driver;
using ResumeBuilder.Infrastructure.Repositories.Users.Models;
using ResumeBuilder.Infrastructure.Repositories.Users;
using Microsoft.OpenApi.Models;
using ResumeBuilder.Infrastructure.Repositories.Resumes;
using ResumeBuilder.Infrastructure.Repositories.Resumes.Models;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;

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

        var rsaKey = RSA.Create();
        rsaKey.ImportRSAPublicKey(Convert.FromBase64String(config["JwtSettings:PublicKey"]!), out _);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie()
            .AddJwtBearer("access", x =>
            {
                x.MapInboundClaims= false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:AccessTokenAudience"],
                    IssuerSigningKey = new RsaSecurityKey(rsaKey),
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };

                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var cookieName = config["JwtSettings:AccessTokenCookieName"]!;
                        var cookieToken = context.Request.Cookies[cookieName];
                        var headerToken = context.Request.Headers[HeaderNames.Authorization];
                        context.Token = cookieToken != null ? cookieToken : headerToken;
                        return Task.CompletedTask;
                    }
                };
            })
            .AddJwtBearer("refresh", x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:RefreshTokenAudience"],
                    IssuerSigningKey = new RsaSecurityKey(rsaKey),
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var cookieName = config["JwtSettings:RefreshTokenCookieName"]!;
                        var cookieToken = context.Request.Cookies[cookieName];
                        var headerToken = context.Request.Headers[HeaderNames.Authorization];
                        context.Token = cookieToken != null ? cookieToken : headerToken;
                        return Task.CompletedTask;
                    }
                };
            });
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n\r\nExample: \"12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            }
        );
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure();
        var MyAllowSpecificOrigins = "_MyAllowSubdomainPolicy";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
        });
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

        app.UseCors(MyAllowSpecificOrigins);

        // This will be added back once we get the ValidTypes thing working
        //IdentityModelEventSource.ShowPII = true;
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
            AddMongoDbRepository<ResumeRepository, ResumeInfra>(config["CareerVentureDatabaseSettings:ResumesCollectionName"]!);

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