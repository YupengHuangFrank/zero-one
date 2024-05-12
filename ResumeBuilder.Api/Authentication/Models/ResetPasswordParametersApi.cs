using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.Api.Authentication.Models
{
    public class ResetPasswordParametersApi : IValidatableObject
    {
        public string Password { get; set; }

        public ResetPasswordParametersApi(string password)
        {
            Password = password;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Password))
                yield return new ValidationResult("Password can't be null");
        }
    }
}
