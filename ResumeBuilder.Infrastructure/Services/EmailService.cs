using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ResumeBuilder.Infrastructure.Clients;

namespace ResumeBuilder.Infrastructure.Services
{
    public interface IEmailService
    {
        void SendEmail(string email, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ISmtpClient _smtpClient;

        public EmailService(IConfiguration configuration, ISmtpClient smtpClient) 
        {
            _configuration = configuration;
            _smtpClient = smtpClient;
        }

        public void SendEmail(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_configuration["Smtp:Email"]!));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };
            var option = (SecureSocketOptions)Enum.Parse(typeof(SecureSocketOptions), _configuration["Smtp:SecureSocketOptions"]!);

            try
            {
                _smtpClient.Connect(_configuration["Smtp:Host"]!, int.Parse(_configuration["Smtp:Port"]!), option);
                _smtpClient.Authenticate(_configuration["Smtp:Email"]!, _configuration["Smtp:Password"]!);
                _smtpClient.Send(emailMessage);
            }
            catch(Exception e)
            {
                throw new Exception("Error sending email.", e);
            }
            finally
            {
                _smtpClient.Disconnect(true);
            }
        }
    }
}
