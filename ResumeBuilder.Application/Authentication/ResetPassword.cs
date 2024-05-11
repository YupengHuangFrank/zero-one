using MediatR;
using Microsoft.AspNetCore.Identity;
using ResumeBuilder.Application.Services;
using ResumeBuilder.Infrastructure.Repositories.Users;

namespace ResumeBuilder.Application.Authentication
{
    public class ResetPassword : IRequestHandler<ResetPasswordRequest, ResetPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IPasswordHasher<string> _passwordHasher;

        public ResetPassword(IUserRepository userRepository, IEncryptionService encryptionService, IPasswordHasher<string> passwordHasher)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResetPasswordResponse> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var email = _encryptionService.Decrypt(request.EncryptedEmail);
            var resetPasswordCode = _encryptionService.Decrypt(request.EncryptedResetPasswordCode);

            var user = await _userRepository.GetUserFromEmail(email);
            if (user is null)
                throw new ArgumentException("Invalid email");

            if (user.ResetPasswordCode != resetPasswordCode)
                throw new ArgumentException("Invalid reset password code");
            
            user.Password = _passwordHasher.HashPassword(string.Empty, request.NewPassword);
            user.ResetPasswordCode = null;
            var result = await _userRepository.UpdateUser(user) == 1 ? true : false;

            return new ResetPasswordResponse(result);
        }
    }

    public class ResetPasswordRequest : IRequest<ResetPasswordResponse>
    {
        public string EncryptedEmail { get; }
        public string EncryptedResetPasswordCode { get; }
        public string NewPassword { get; }

        public ResetPasswordRequest(string encryptedEmail, string encryptedResetPasswordCode, string newPassword)
        {
            EncryptedEmail = encryptedEmail;
            EncryptedResetPasswordCode = encryptedResetPasswordCode;
            NewPassword = newPassword;
        }
    }

    public class ResetPasswordResponse
    {
        public bool Success { get; }

        public ResetPasswordResponse(bool success)
        {
            Success = success;
        }
    }
}
