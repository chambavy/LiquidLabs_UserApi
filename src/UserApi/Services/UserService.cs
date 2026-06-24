using Microsoft.Data.SqlClient;
using UserApi.Models;
using UserApi.ExternalApis;
using UserApi.Repositories;

namespace UserApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JsonPlaceholderClient _jsonPlaceHolderClient;
    private readonly ILogger<UserService> _logger;
    public UserService(IConfiguration configuration, IUserRepository userRepository,JsonPlaceholderClient jsonPlaceHolderClient, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _jsonPlaceHolderClient = jsonPlaceHolderClient;
        _logger = logger;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
            _logger.LogInformation("Fetching all users...");
            var users = await _userRepository.GetAllUsersAsync();

            if (users.Count > 0)
                return users;
            _logger.LogInformation("Users not found in DB, Fetching users from External API...");
            users = await _jsonPlaceHolderClient.GetAllUsersAsync();
            _logger.LogInformation("Saving Users to the DB...");
            await _userRepository.SaveUsersAsync(users);
            _logger.LogInformation("Retrieved {Count} users", users.Count);
            return users ?? new List<User>();
        
    }
    public async Task<User?> GetUserByIdAsync(int Id)
    {
        
            _logger.LogInformation("Fetching user {Id} from DB...", Id);
            var user = await _userRepository.GetUserByIdAsync(Id);

            if (user != null)
                return user;
            _logger.LogInformation("User not found in DB, Fetching user {Id} from External API...", Id);
            user = await _jsonPlaceHolderClient.GetUserByIdAsync(Id);
            if (user == null)
                return null;

            _logger.LogInformation("Saving user {Id} to DB...", Id);

            await _userRepository.SaveUserAsync(user);

            return user;
    }
}