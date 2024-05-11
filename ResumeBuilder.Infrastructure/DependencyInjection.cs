using Microsoft.Extensions.DependencyInjection;
using ResumeBuilder.Infrastructure.Clients;
using ResumeBuilder.Infrastructure.Repositories.Resumes;
using ResumeBuilder.Infrastructure.Repositories.Users;
using ResumeBuilder.Infrastructure.Services;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ResumeBuilder.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services) 
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IResumeRepository, ResumeRepository>();
            services.AddSingleton<ISmtpClient, SmtpClient>();
            services.AddSingleton<IEmailService, EmailService>();
        }
    }
}
