using AutoMapper;
using ResumeBuilder.Api.Authentication.Models;
using ResumeBuilder.Api.Resumes.Models;
using ResumeBuilder.Api.Users.Models;
using ResumeBuilder.Domain.Resumes;
using System.Diagnostics.CodeAnalysis;

namespace ResumeBuilder.Api
{
    [ExcludeFromCodeCoverage]
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserApi, Domain.Users.User>().ConstructUsing(x => new Domain.Users.User(x.Email, x.FirstName, x.LastName, x.Password, false));
            CreateMap<TokenRequestApi, Domain.Authentication.TokenRequest>();
            CreateMap<EducationApi, Education>();
            CreateMap<HeaderApi, Header>();
            CreateMap<ResumeApi, Resume>();
            CreateMap<TemplateApi, Template>();
            CreateMap<WorkExperienceApi, WorkExperience>();
            CreateMap<AddressApi, Address>();
        }
    }
}
