using MediatR;
using ResumeBuilder.Infrastructure.Repositories.Users;
using ResumeBuilder.Infrastructure.Services;

namespace ResumeBuilder.Application.Authentication
{
    public class ForgetPassword : IRequestHandler<ForgetPasswordRequest, ForgetPasswordResponse>
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public ForgetPassword(IEmailService emailService, IUserRepository userRepository)
        {
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<ForgetPasswordResponse> Handle(ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetUserFromEmail(request.Email!);
            if (user is null)
                throw new ArgumentException(nameof(ForgetPasswordRequest.Email));

            var verificationCode = GenerateVerificationCode();
            await _userRepository.SetResetPasswordCode(request.Email!, verificationCode);
            _emailService.SendEmail(request.Email!, "Reset your password", $"Here is your verification code {verificationCode}");
            return new ForgetPasswordResponse();
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string verificationCode = "";

            for (int i = 0; i < 6; i++) // Generate a code of 6 characters
            {
                int position = random.Next(characters.Length); // Pick a random position from the characters string
                verificationCode += characters[position]; // Append the character at the picked position
            }

            return verificationCode;
        }
    }

    public class ForgetPasswordRequest : IRequest<ForgetPasswordResponse>
    {
        public string? Email { get; }

        public ForgetPasswordRequest(string email)
        {
            Email = email;
        }
    }

    public class ForgetPasswordResponse
    {
    }
}
