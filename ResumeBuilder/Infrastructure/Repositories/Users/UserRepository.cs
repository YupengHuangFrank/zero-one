using ResumeBuilder.Domain.Models;
using ResumeBuilder.Infrastructure.Repositories.Users.Models;

namespace ResumeBuilder.Infrastructure.Repositories.Users
{
    public interface IUserRepository
    {
        void CreateUser(User user);
    }

    public class UserRepository : IUserRepository
    {
        public void CreateUser(User user)
        {
            Console.WriteLine("User created");
        }
    }
}
