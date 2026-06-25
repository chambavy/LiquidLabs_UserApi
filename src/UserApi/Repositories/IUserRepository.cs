using UserApi.Models;
public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync(int? pageNumber, int? pageSize);
    Task<User?> GetUserByIdAsync(int Id);
    Task SaveUsersAsync(List<User> users);
    Task SaveUserAsync(User user);
    Task<int> GetUserCountAsync();
}