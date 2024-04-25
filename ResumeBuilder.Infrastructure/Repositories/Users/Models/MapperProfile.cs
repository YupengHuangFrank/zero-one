using AutoMapper;
using ResumeBuilder.Domain.Resumes;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Resumes.Models;
using System.Diagnostics.CodeAnalysis;

namespace ResumeBuilder.Infrastructure.Repositories.Users.Models
{
    [ExcludeFromCodeCoverage]
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserInfra>();
            CreateMap<Education, EducationInfra>();
            CreateMap<Header, HeaderInfra>();
            CreateMap<Resume, ResumeInfra>();
            CreateMap<Template, TemplateInfra>();
            CreateMap<WorkExperience, WorkExperienceInfra>();
            CreateMap<Address, AddressInfra>();
            CreateMap<UserInfra, User>();
            CreateMap<EducationInfra, Education>();
            CreateMap<HeaderInfra, Header>();
            CreateMap<ResumeInfra, Resume>();
            CreateMap<TemplateInfra, Template>();
            CreateMap<WorkExperienceInfra, WorkExperience>();
            CreateMap<AddressInfra, Address>();

        }
    }
}
