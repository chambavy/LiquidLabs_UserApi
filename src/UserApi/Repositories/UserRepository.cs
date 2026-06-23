using Microsoft.Data.SqlClient;
using UserApi.Models;

namespace UserApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string is missing."); 
    }
    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = new List<User>();
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"SELECT UserId,ExternalId,Name,UserName,Email,CreatedAt
                                             FROM Users
                                             ORDER BY ExternalId", connection);
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var user = new User
            {
                UserId = reader.GetInt32(0),
                ExternalId = reader.GetInt32(1),
                Name = reader.GetString(2),
                Username = reader.GetString(3),
                Email = reader.GetString(4),
                CreatedAt = reader.GetDateTime(5)
            };
            users.Add(user);
        }
        return users;
    }

    public async Task<User> GetUserById(int ExternalId)
    {
        var User = new User();
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"SELECT UserId,ExternalId,Name,UserName,Email,CreatedAt
                                             FROM Users
                                             WHERE ExternalId = @ExternalId", connection);
        command.Parameters.AddWithValue("@ExternalId", ExternalId);
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            User.UserId = reader.GetInt32(0);
            User.ExternalId = reader.GetInt32(1);
            User.Name = reader.GetString(2);
            User.Username = reader.GetString(3);
            User.Email = reader.GetString(4);
            User.CreatedAt = reader.GetDateTime(5);
        }
        return User;
    }
}