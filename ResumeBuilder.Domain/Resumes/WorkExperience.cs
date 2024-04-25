namespace ResumeBuilder.Domain.Resumes
{
    public class WorkExperience
    {
        public string? JobTitle { get; set; }
        public string? Company { get; set; }
        public Address? Address { get; set; }
        public int? StartMonth { get; set; }
        public int? EndMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public string? Description { get; set; }
    }
}
