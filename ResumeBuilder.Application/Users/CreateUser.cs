using MediatR;
using Microsoft.AspNetCore.Identity;
using ResumeBuilder.Application.Services;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Users;
using ResumeBuilder.Infrastructure.Services;

namespace ResumeBuilder.Application.Users
{
    public class CreateUser : IRequestHandler<CreateUserRequest, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly IEncryptionService _encryptionService;

        public CreateUser(IUserRepository userRepository, IPasswordHasher<string> passwordHasher, IEmailService emailService, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _encryptionService = encryptionService;
        }

        public async Task<CreateUserResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                request.User.Password = _passwordHasher.HashPassword(string.Empty, request.User.Password!);
                var result = await _userRepository.CreateUser(request.User);
                if (result > 0)
                {
                    var encryptedEmail = _encryptionService.Encrypt(request.User.Email!);
                    var confirmEmailUri = request.ConfirmEmailUri + "?email=" + encryptedEmail;
                    // Send email with encryptedUserEmail
                    _emailService.SendEmail(request.User.Email!, "Verify your email", confirmEmailUri);
                }
                return new CreateUserResult(result);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating user.", e);
            }
        }
    }

    public class CreateUserRequest : IRequest<CreateUserResult>
    {
        public User User { get; }
        public string ConfirmEmailUri { get; }

        public CreateUserRequest(User user, string confirmEmailUri)
        {
            User = user;
            ConfirmEmailUri = confirmEmailUri;
        }
    }

    public class CreateUserResult
    {
        public int NumberOfUserCreated { get; }
        public CreateUserResult(int numberOfUserCreated)
        {
            NumberOfUserCreated = numberOfUserCreated;
        }
    }
}
