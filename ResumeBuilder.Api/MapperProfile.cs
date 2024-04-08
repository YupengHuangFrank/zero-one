using AutoMapper;
using ResumeBuilder.Api.Authentication.Models;
using ResumeBuilder.Api.Users.Models;
using System.Diagnostics.CodeAnalysis;

namespace ResumeBuilder.Api
{
    [ExcludeFromCodeCoverage]
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserApi, Domain.Users.User>().ConstructUsing(x => new Domain.Users.User(x.Email, x.FirstName, x.LastName, x.Password));
            CreateMap<TokenRequestApi, Domain.Authentication.TokenRequest>();
        }
    }
}
