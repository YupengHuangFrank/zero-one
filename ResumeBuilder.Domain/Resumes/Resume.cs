namespace ResumeBuilder.Domain.Resumes
{
    public class Resume
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public Template? Template { get; set; }
        public Header? Header { get; set; }
        public IEnumerable<WorkExperience>? WorkExperience { get; set; }
        public IEnumerable<Education>? Education { get; set; }
        public string? Summary { get; set; }
    }
}
