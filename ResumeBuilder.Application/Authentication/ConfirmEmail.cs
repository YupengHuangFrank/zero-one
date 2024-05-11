using MediatR;
using ResumeBuilder.Application.Services;
using ResumeBuilder.Infrastructure.Repositories.Users;

namespace ResumeBuilder.Application.Authentication
{
    public class ConfirmEmail : IRequestHandler<ConfirmEmailRequest, ConfirmEmailResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;

        public ConfirmEmail(IUserRepository userRepository, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<ConfirmEmailResponse> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var userEmail = _encryptionService.Decrypt(request.Token);
            var user = await _userRepository.GetUserFromEmail(userEmail);
            if (user == null)
                throw new Exception("User not found.");

            user.IsVerified = true;
            var acknowledged = await _userRepository.UpdateUser(user);
            if (acknowledged == 0)
                throw new Exception("Error confirming email.");

            return new ConfirmEmailResponse(true);
        }
    }

    public class ConfirmEmailRequest : IRequest <ConfirmEmailResponse>
    {
        public string Token { get; }

        public ConfirmEmailRequest(string token)
        {
            Token = token;
        }
    }

    public class ConfirmEmailResponse
    {
        public bool Success { get; set; }

        public ConfirmEmailResponse(bool success)
        {
            Success = success;
        }
    }
}
