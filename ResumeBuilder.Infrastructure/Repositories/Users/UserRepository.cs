using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Users.Models;

namespace ResumeBuilder.Infrastructure.Repositories.Users
{
    public interface IUserRepository
    {
        int CreateUser(User user);
        string GetHashedPassword(string email);
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

        public int CreateUser(User user)
        {
            var userInfra = _mapper.Map<UserInfra>(user);
            if (_userCollection.Find(x => x.Email == user.Email).Any())
                return 0;

            _userCollection.InsertOne(userInfra);
            return 1;
        }

        public string GetHashedPassword(string email)
        {
            var userInfra = _userCollection.Find(x => x.Email == email).FirstOrDefault();
            return userInfra?.Password ?? "";
        }
    }
}
