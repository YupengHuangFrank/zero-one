using AutoMapper;
using MediatR;
using ResumeBuilder.Domain.Models;
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
            _userRepository.CreateUser(request.User);
            return Task.FromResult(new CreateUserResult());
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
