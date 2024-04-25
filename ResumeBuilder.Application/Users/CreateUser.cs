using MediatR;
using Microsoft.AspNetCore.Identity;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Users;

namespace ResumeBuilder.Application.Users
{
    public class CreateUser : IRequestHandler<CreateUserRequest, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<string> _passwordHasher;

        public CreateUser(IUserRepository userRepository, IPasswordHasher<string> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<CreateUserResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                request.User.Password = _passwordHasher.HashPassword(string.Empty, request.User.Password!);
                var result = await _userRepository.CreateUser(request.User);
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
        
        public CreateUserRequest(User user)
        {
            User = user;
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
