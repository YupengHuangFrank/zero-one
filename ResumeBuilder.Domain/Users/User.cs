namespace ResumeBuilder.Domain.Users
{
    public class User
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public bool? IsVerified { get; set; }
        public UserType UserType { get; set; } = UserType.User;
        public string? ResetPasswordCode { get; set; }
        public IEnumerable<string> ResumeIds { get; set; } = new List<string>();

        public User(string? email, string? firstName, string? lastName, string? password, bool? isVerified)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            IsVerified = isVerified;
        }
    }
}
