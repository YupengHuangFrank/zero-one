using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserRepository(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;          
        }   

        public int CreateUser(User user)
        {
            var userInfra = _mapper.Map<UserInfra>(user);
            var result = 0;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
                    BEGIN
                        INSERT INTO Users (UserId, Email, FirstName, LastName, Password) VALUES (@UserId, @Email, @FirstName, @LastName, @Password)
                    END
                    ";
                command.Parameters.AddWithValue("@UserId", userInfra.UserId);
                command.Parameters.AddWithValue("@Email", userInfra.Email);
                command.Parameters.AddWithValue("@FirstName", userInfra.FirstName);
                command.Parameters.AddWithValue("@LastName", userInfra.LastName);
                command.Parameters.AddWithValue("@Password", userInfra.Password);
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public string GetHashedPassword(string email)
        {
            string result = "";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TOP 1 Password FROM Users WHERE Email = @Email
                    ";
                command.Parameters.AddWithValue("@Email", email);
                var reader = command.ExecuteReader();
                while (reader.Read())
                    result = reader.GetString(0);
            }
            return result;
        }
    }
}
