using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.Api.Resumes.Models
{
    public class EducationApi : IValidatableObject
    {
        public string? School { get; set; }
        public string? Degree { get; set; }
        public string? Field { get; set; }
        public int? GraduationMonth { get; set; }
        public int? GraduationYear { get; set; }
        public AddressApi? Address { get; set; }
        public double GPA { get; set; }
        public IEnumerable<string>? Courses { get; set; }
        public IEnumerable<string>? Honors { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (GraduationMonth != null && (GraduationMonth < 1 || GraduationMonth > 12))
                yield return new ValidationResult("GraduationMonth in EducationApi must be between 1 and 12.");
        }
    }
}
