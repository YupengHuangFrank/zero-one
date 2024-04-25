namespace ResumeBuilder.Api.Resumes.Models
{
    public class HeaderApi
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressApi? Address { get; set; }
    }
}
