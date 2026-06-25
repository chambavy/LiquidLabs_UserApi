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
    public async Task<List<User>> GetAllUsersAsync(int? pageNumber, int? pageSize)
    {
        var users = new List<User>();
        using var connection = new SqlConnection(_connectionString);
        string query;
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = @"
        SELECT Id,Name,UserName,Email,CreatedAt
        FROM Users
        ORDER BY Id
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";
        }
        else
        {
            query = @"
        SELECT Id,Name,UserName,Email,CreatedAt
        FROM Users
        ORDER BY Id";
        }
        using var command = new SqlCommand(query, connection);
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var offset = (pageNumber - 1) * pageSize;
            command.Parameters.Add("@Offset", SqlDbType.Int).Value = offset;
            command.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
        }
            

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var user = new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
            users.Add(user);
        }
        return users;
    }

    public async Task<User?> GetUserByIdAsync(int Id)
    {
        using var connection = new SqlConnection(_connectionString);

        using var command = new SqlCommand(@"
        SELECT Id, Name, UserName, Email, CreatedAt
        FROM Users
        WHERE Id = @Id", connection);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new User
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Username = reader.GetString(reader.GetOrdinal("Username")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
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

        command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = user.Id;
        command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = user.Name;
        command.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar).Value = user.Username;
        command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar).Value = user.Email;

        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> GetUserCountAsync()
    {
        using var connection = new SqlConnection(_connectionString);

        using var command = new SqlCommand(
            "SELECT COUNT(*) FROM Users",
            connection);

        await connection.OpenAsync();

        return (int)await command.ExecuteScalarAsync();
    }
}