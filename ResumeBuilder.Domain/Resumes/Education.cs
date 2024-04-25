namespace ResumeBuilder.Domain.Resumes
{
    public class Education
    {
        public string? School { get; set; }
        public string? Degree { get; set; }
        public string? Field { get; set; }
        public int? GraduationMonth { get; set; }
        public int? GraduationYear { get; set; }
        public Address? Address { get; set; }
        public double GPA { get; set; }
        public IEnumerable<string>? Courses { get; set; }
        public IEnumerable<string>? Honors { get; set; }
    }
}
