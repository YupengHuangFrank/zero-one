using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResumeBuilder.Infrastructure.Repositories.Resumes;
using ResumeBuilder.Infrastructure.Repositories.Users;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ResumeBuilder.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IResumeRepository, ResumeRepository>();
        }
    }
}
