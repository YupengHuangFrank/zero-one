﻿namespace ResumeBuilder.Domain.Users
{
    public class User
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }

        public User(string? email, string? firstName, string? lastName, string? password)
        {
            UserId = Guid.NewGuid();
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
        }
    }
}