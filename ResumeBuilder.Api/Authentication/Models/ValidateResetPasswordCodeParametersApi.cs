using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.Api.Authentication.Models
{
    public class ValidateResetPasswordCodeParametersApi : IValidatableObject
    {
        public string? Email { get; set; }
        public string? ResetPasswordCode { get; set; }

        public ValidateResetPasswordCodeParametersApi(string email, string resetPasswordCode)
        {
            Email = email;
            ResetPasswordCode = resetPasswordCode;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Email == null)
                yield return new ValidationResult("Email in ValidateResetPasswordParametersApi is null");

            if (ResetPasswordCode == null)
                yield return new ValidationResult("ResetPasswordCode in ValidateResetPasswordParametersApi is null");
        }
    }
}
