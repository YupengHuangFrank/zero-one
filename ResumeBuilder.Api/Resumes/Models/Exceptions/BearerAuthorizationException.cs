namespace ResumeBuilder.Api.Resumes.Models.Exceptions
{
    public class BearerAuthorizationException : Exception
    {
        public BearerAuthorizationException(string message) : base(message)
        {
        }
    }
}
