using UserApi.Models;
public interface IUserService
{
    Task<PagedResponse<List<User>>> GetAllUsersAsync(int? pageNumber,int? pageSize);
    Task<User?> GetUserByIdAsync(int Id);
}