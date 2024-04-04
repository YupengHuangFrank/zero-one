using MediatR;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Users;

namespace ResumeBuilder.Application.Users
{
    public class CreateUser : IRequestHandler<CreateUserRequest, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;

        public CreateUser(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<CreateUserResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _userRepository.CreateUser(request.User);
                return Task.FromResult(new CreateUserResult());
            }
            catch (Exception e)
            {
                throw new Exception("Error creating user", e);
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
        
    }
}
