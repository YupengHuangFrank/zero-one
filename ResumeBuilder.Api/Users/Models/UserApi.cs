﻿using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ResumeBuilder.Api.Users.Models
{
    public class UserApi : IValidatableObject
    {
        [Required, MinLength(1)]
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required, MinLength(1)]
        public string? Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email))
                yield return new ValidationResult("Email in UserApi cannot be null.");
            else
                if (!Regex.IsMatch(Email, "^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$"))
                    yield return new ValidationResult($"Email in UserApi is invalid.");

            if (string.IsNullOrWhiteSpace(Password))
                yield return new ValidationResult("Password in UserApi cannot be null.");            
        }
    }
}
