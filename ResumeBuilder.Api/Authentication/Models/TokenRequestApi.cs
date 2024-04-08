using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ResumeBuilder.Api.Authentication.Models
{
    public class TokenRequestApi : IValidatableObject
    {
        public string? Email { get; set; }
        public string? Password { get; set; }

        public TokenRequestApi(string? email, string? password)
        {
            Email = email;
            Password = password;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validations = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(Email))
                validations.Add(new ValidationResult("Email in TokenRequestApi is null"));
            else
                if (!Regex.IsMatch(Email, "^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$"))
                validations.Add(new ValidationResult($"Email in TokenRequestApi is not formatted correctly"));

            if (string.IsNullOrWhiteSpace(Password))
                validations.Add(new ValidationResult("Password in TokenRequestApi is null"));

            return validations;
        }
    }
}
