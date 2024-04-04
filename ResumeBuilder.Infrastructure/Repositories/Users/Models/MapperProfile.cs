using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace ResumeBuilder.Infrastructure.Repositories.Users.Models
{
    [ExcludeFromCodeCoverage]
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Domain.Users.User, UserInfra>();
        }
    }
}
