using Microsoft.Data.SqlClient;
using UserApi.Models;
using UserApi.ExternalApis;
using UserApi.Repositories;

namespace UserApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JsonPlaceHolderClient _jsonPlaceHolderClient;
    private readonly ILogger<UserService> _logger;
    public UserService(IConfiguration configuration, IUserRepository userRepository,JsonPlaceHolderClient jasonClient,ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _jsonPlaceHolderClient = jasonClient;
        _logger = logger;
    }
    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = new List<User>();
        _logger.LogInformation("Fetching users from DB");
        users = await _userRepository.GetAllUsersAsync();

        if (users.Count == 0) {
         _logger.LogInformation("Database is empty. Fetching users from external API.");
         users = await _jsonPlaceHolderClient.GetAllUsersAsync();
        }
        _logger.LogInformation("Retrived{Users.Count} from DB", users.Count);
        return users;
    }

    public async Task<User> GetUserByIdAsync(int Id)
    {
        _logger.LogInformation("Fetching user with Id {Id}", Id);
        var User = await _userRepository.GetUserByIdAsync(Id);
        if (User.Id == 0)
        {
            _logger.LogInformation("User {Id} not found in db", Id);
            _logger.LogInformation("Fetching user {Id} from external api", Id);
            User =await _jsonPlaceHolderClient.GetUserByIdAsync(Id);
        }
        return User;
    }
}