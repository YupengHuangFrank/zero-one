namespace ResumeBuilder.Domain.Authentication
{
    public class TokenRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }

        public TokenRequest(string? email, string? password)
        {
            Email = email;
            Password = password;
        }
    }
}
