using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.Api.Authentication.Models
{
    public class RefreshAccessTokenApi : IValidatableObject
    {
        public string? RefreshToken { get; set; }

        public RefreshAccessTokenApi(string? refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(RefreshToken))
                yield return new ValidationResult("RefreshToken in RefreshAccessTokenApi is null");
        }
    }
}
