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
    public UserService(IUserRepository userRepository,JsonPlaceholderClient jsonPlaceHolderClient, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _jsonPlaceHolderClient = jsonPlaceHolderClient;
        _logger = logger;
    }

    public async Task<PagedResponse<List<User>>> GetAllUsersAsync(int? pageNumber,int? pageSize)
    {
        var count = await _userRepository.GetUserCountAsync();

        if (count == 0)
        {
            _logger.LogInformation("Users not found in DB, Fetching users from External API...");
            var apiUsers = await _jsonPlaceHolderClient.GetAllUsersAsync();
            _logger.LogInformation("Saving Users to the DB...");
            await _userRepository.SaveUsersAsync(apiUsers);
            count = apiUsers.Count;
        }
        _logger.LogInformation("Fetching all users...");
        var users = await _userRepository.GetAllUsersAsync(pageNumber,pageSize);
        _logger.LogInformation("Retrieved users");
        return new PagedResponse<List<User>>
        {
            Success = true,
            Data = users,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = count
        };
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
            _logger.LogInformation("Fetching user {Id} from DB...", Id);
            return await _userRepository.GetUserByIdAsync(Id);
    }
}