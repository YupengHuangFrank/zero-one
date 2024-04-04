using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace ResumeBuilder.Api.Users.Models
{
    [ExcludeFromCodeCoverage]
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserApi, Domain.Users.User>().ConstructUsing(x => new Domain.Users.User(x.Email, x.FirstName, x.LastName, x.Password));
        }
    }
}
