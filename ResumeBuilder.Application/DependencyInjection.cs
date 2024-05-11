using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResumeBuilder.Application.Services;
using ResumeBuilder.Infrastructure.Repositories.Users;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ResumeBuilder.Application
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();
            services.AddSingleton<IEncryptionService, EncryptionService>();
        }
    }
}
