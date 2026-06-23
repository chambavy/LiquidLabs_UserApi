using UserApi.Models;
public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserById(int ExternalId);
}