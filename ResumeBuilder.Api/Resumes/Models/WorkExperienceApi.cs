using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.Api.Resumes.Models
{
    public class WorkExperienceApi : IValidatableObject
    {
        public string? JobTitle { get; set; }
        public string? Company { get; set; }
        public AddressApi? Address { get; set; }
        public int? StartMonth { get; set; }
        public int? EndMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public string? Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartMonth != null && (StartMonth < 1 || StartMonth > 12))
                yield return new ValidationResult("StartMonth must be between 1 and 12.");
            if (EndMonth != null && (EndMonth < 1 || EndMonth > 12))
                yield return new ValidationResult("EndMonth must be between 1 and 12.");
            if (StartYear != null && StartYear > DateTime.UtcNow.Year)
                yield return new ValidationResult("StartYear cannot be in the future.");
            if (EndYear != null && EndYear > DateTime.UtcNow.Year)
                yield return new ValidationResult("EndYear cannot be in the future.");
        }
    }
}
