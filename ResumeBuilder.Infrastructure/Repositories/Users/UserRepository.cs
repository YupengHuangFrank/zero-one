using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ResumeBuilder.Domain.Users;

namespace ResumeBuilder.Infrastructure.Repositories.Users
{
    public interface IUserRepository
    {
        void CreateUser(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }   

        public void CreateUser(User user)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Users (UserId, Email, FirstName, LastName, Password) VALUES (@UserId, @Email, @FirstName, @LastName, @Password)";
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
