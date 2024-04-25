namespace ResumeBuilder.Infrastructure.Repositories.Resumes.Models
{
    public class EducationInfra
    {
        public string? School { get; set; }
        public string? Degree { get; set; }
        public string? Field { get; set; }
        public int? GraduationMonth { get; set; }
        public int? GraduationYear { get; set; }
        public AddressInfra? Address { get; set; }
        public double GPA { get; set; }
        public IEnumerable<string>? Courses { get; set; }
        public IEnumerable<string>? Honors { get; set; }
    }
}
