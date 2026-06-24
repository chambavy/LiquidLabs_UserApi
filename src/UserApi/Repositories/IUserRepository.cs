using UserApi.Models;
public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int Id);
    Task SaveUsersAsync(List<User> users);
    Task SaveUserAsync(User user);
}