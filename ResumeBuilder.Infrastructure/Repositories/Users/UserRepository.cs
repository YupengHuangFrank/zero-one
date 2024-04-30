using AutoMapper;
using MongoDB.Driver;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Users.Models;

namespace ResumeBuilder.Infrastructure.Repositories.Users
{
    public interface IUserRepository
    {
        Task<int> CreateUser(User user);
        Task<int> UpdateUser(User user);
        Task<User?> GetUserFromEmail(string email);
        Task<User?> GetUser(string id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserInfra> _userCollection;
        private readonly IMapper _mapper;

        public UserRepository(IMongoCollection<UserInfra> userCollection, IMapper mapper)
        {
            _userCollection = userCollection;
            _mapper = mapper;
        }

        public async Task<int> CreateUser(User user)
        {
            var userInfra = _mapper.Map<UserInfra>(user);
            if (_userCollection.Find(x => x.Email == user.Email).Any())
                return 0;

            await _userCollection.InsertOneAsync(userInfra);
            return 1;
        }

        public async Task<int> UpdateUser(User user)
        {
            var userInfra = _mapper.Map<UserInfra>(user);
            var result = await _userCollection.ReplaceOneAsync(x => x.Id == userInfra.Id, userInfra);
            return result.IsAcknowledged ? 1 : 0;
        }

        public async Task<User?> GetUserFromEmail(string email)
        {
            var userInfras = await _userCollection.FindAsync(x => x.Email == email);
            var userInfra = userInfras.FirstOrDefault();
            if (userInfra == null)
                return null;

            var user = new User(userInfra.Email!, userInfra.FirstName!, userInfra.LastName!, userInfra.Password!)
            {
                Id = userInfra.Id,
                ResumeIds = userInfra.ResumeIds
            };
            return user;
        }

        public async Task<User?> GetUser(string id)
        {
            var userInfras = await _userCollection.FindAsync(x => x.Id == id);
            var userInfra = userInfras.FirstOrDefault();
            if (userInfra == null)
                return null;

            var user = new User(userInfra.Email!, userInfra.FirstName!, userInfra.LastName!, userInfra.Password!) 
            { 
                Id = userInfra.Id,
                ResumeIds = userInfra.ResumeIds
            };
            return user;
        }
    }
}
