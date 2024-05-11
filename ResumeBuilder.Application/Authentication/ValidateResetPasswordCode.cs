using MediatR;
using ResumeBuilder.Application.Services;
using ResumeBuilder.Infrastructure.Repositories.Users;

namespace ResumeBuilder.Application.Authentication
{
    public class ValidateResetPasswordCode : IRequestHandler<ValidateResetPasswordCodeRequest, ValidateResetPasswordCodeResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;

        public ValidateResetPasswordCode(IUserRepository userRepository, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<ValidateResetPasswordCodeResponse> Handle(ValidateResetPasswordCodeRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserFromEmail(request.Email);
            if (user == null)
                throw new ArgumentException(nameof(request.Email));

            if (request.ResetPasswordCode == null)                
                throw new ArgumentException(nameof(request.ResetPasswordCode));

            if (user.ResetPasswordCode != request.ResetPasswordCode)
            {
                user.ResetPasswordCode = null;
                await _userRepository.UpdateUser(user);
                return new ValidateResetPasswordCodeResponse(string.Empty);
            }

            var path = "email=" + _encryptionService.Encrypt(request.Email) + "&resetPasswordCode=" + _encryptionService.Encrypt(request.ResetPasswordCode);
            return new ValidateResetPasswordCodeResponse(path);
        }
    }

    public class ValidateResetPasswordCodeRequest : IRequest<ValidateResetPasswordCodeResponse>
    {
        public string Email { get; }
        public string ResetPasswordCode { get; }

        public ValidateResetPasswordCodeRequest(string email, string verificationCode) 
        {
            Email = email;
            ResetPasswordCode = verificationCode;
        }
    }

    public class ValidateResetPasswordCodeResponse
    {
        public string ResetPasswordPath { get; }

        public ValidateResetPasswordCodeResponse(string resetPasswordPath)
        {
            ResetPasswordPath = resetPasswordPath;
        }
    }
}
