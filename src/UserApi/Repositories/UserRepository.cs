using Microsoft.Data.SqlClient;
using UserApi.Models;
using System.Data;

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
        using var command = new SqlCommand(@"SELECT Id,Name,UserName,Email,CreatedAt
                                             FROM Users
                                             ORDER BY Id", connection);
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var user = new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Username = reader.GetString(2),
                Email = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4)
            };
            users.Add(user);
        }
        return users;
    }

    public async Task<User> GetUserByIdAsync(int Id)
    {
        var User = new User();
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"SELECT Id,Name,UserName,Email,CreatedAt
                                             FROM Users
                                             WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", Id);
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            User.Id = reader.GetInt32(0);
            User.Name = reader.GetString(1);
            User.Username = reader.GetString(2);
            User.Email = reader.GetString(3);
            User.CreatedAt = reader.GetDateTime(4);
        }
        return User;
    }

    public async Task SaveUsersAsync(List<User> users)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Username", typeof(string));
        table.Columns.Add("Email", typeof(string));
        table.Columns.Add("CreatedAt", typeof(DateTime));

        foreach (var u in users)
        {
            table.Rows.Add(u.Id, u.Name, u.Username, u.Email, DateTime.UtcNow);
        }

        using var bulkCopy = new SqlBulkCopy(connection);
        bulkCopy.DestinationTableName = "Users";

        await bulkCopy.WriteToServerAsync(table);
    }
    public async Task SaveUserAsync(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(@"
        IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @Id)
        BEGIN
            INSERT INTO Users (Id, Name, Username, Email, CreatedAt)
            VALUES (@Id, @Name, @Username, @Email, GETDATE())
        END", connection);

        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@Email", user.Email);

        await command.ExecuteNonQueryAsync();
    }
}