using MailKit.Security;
using MimeKit;

namespace ResumeBuilder.Infrastructure.Clients
{
    public interface ISmtpClient
    {
        void Connect(string host, int port, SecureSocketOptions secureSocketOption);
        void Authenticate(string email, string password);
        void Send(MimeMessage email);
        void Disconnect(bool quit);
    }

    public class SmtpClient : ISmtpClient
    {
        private readonly MailKit.Net.Smtp.SmtpClient _smtpClient;

        public SmtpClient()
        {
            _smtpClient = new MailKit.Net.Smtp.SmtpClient();
        }

        public void Connect(string host, int port, SecureSocketOptions secureSocketOption)
        {
            //_smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            _smtpClient.Connect(host, port, secureSocketOption);
        }

        public void Authenticate(string email, string password)
        {
            _smtpClient.Authenticate(email, password);
        }

        public void Send(MimeMessage email)
        {
            _smtpClient.Send(email);
        }

        public void Disconnect(bool quit)
        {
            _smtpClient.Disconnect(quit);
        }
    }
}
