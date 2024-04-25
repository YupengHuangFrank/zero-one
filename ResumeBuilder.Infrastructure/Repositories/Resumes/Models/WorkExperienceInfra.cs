namespace ResumeBuilder.Infrastructure.Repositories.Resumes.Models
{
    public class WorkExperienceInfra
    {
        public string? JobTitle { get; set; }
        public string? Company { get; set; }
        public AddressInfra? Address { get; set; }
        public int? StartMonth { get; set; }
        public int? EndMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public string? Description { get; set; }
    }
}
