namespace ResumeBuilder.Api.Resumes.Models
{
    public class ResumeApi
    {
        public string? Id { get; set; }
        public TemplateApi? Template { get; set; }
        public HeaderApi? Header { get; set; }
        public List<WorkExperienceApi>? WorkExperience { get; set; }
        public List<EducationApi>? Education { get; set; }
        public string? Summary { get; set; }
    }
}
