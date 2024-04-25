using MediatR;
using ResumeBuilder.Domain.Resumes;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Resumes;
using ResumeBuilder.Infrastructure.Repositories.Users;

namespace ResumeBuilder.Application.Resumes
{
    public class SaveResume : IRequestHandler<SaveResumeRequest, SaveResumeResult>
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IUserRepository _userRepository;

        public SaveResume(IResumeRepository resumeRepository, IUserRepository userRepository)
        {
            _resumeRepository = resumeRepository;
            _userRepository = userRepository;
        }

        public async Task<SaveResumeResult> Handle(SaveResumeRequest request, CancellationToken cancellationToken)
        {
            var resumeId = await _resumeRepository.AddOrUpdateResume(request.Resume);
            var user = request.User;
            if (resumeId == null)
                return new SaveResumeResult() { Success = false };

            if (!user.ResumeIds.Contains(resumeId))
            {
                user.ResumeIds = user.ResumeIds.Append(resumeId);
                await _userRepository.UpdateUser(user);
            }

            return new SaveResumeResult() { Success = true };
        }
    }

    public class SaveResumeRequest : IRequest<SaveResumeResult>
    {
        public User User { get; set; }
        public Resume Resume { get; set; }

        public SaveResumeRequest(User user, Resume resume)
        {
            User = user;
            Resume = resume;
        }
    }
    
    public class SaveResumeResult
    {
        public bool Success { get; set; }
    }
}
