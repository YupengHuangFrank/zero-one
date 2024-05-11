using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ResumeBuilder.Application.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string text);
        string Decrypt(string text);
    }

    public class EncryptionService : IEncryptionService
    {
        private readonly IConfiguration _configuration;

        public EncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Encrypt(string text)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                var textBytes = Encoding.UTF8.GetBytes(text);
                rsa.FromXmlString(_configuration["Verification:PublicKey"]!);
                var encryptedBytes = rsa.Encrypt(textBytes, false);
                return HttpUtility.UrlEncode(encryptedBytes);
            }
        }

        public string Decrypt(string text)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                var encryptedTextBytes = HttpUtility.UrlDecodeToBytes(text);
                rsa.FromXmlString(_configuration["Verification:PrivateKey"]!);
                var decryptedText = rsa.Decrypt(encryptedTextBytes, false);
                return Encoding.UTF8.GetString(decryptedText);
            }
        }
    }
}
