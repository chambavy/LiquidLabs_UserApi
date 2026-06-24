using Microsoft.AspNetCore.Mvc;
using UserApi.Services;
using UserApi.Models;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{  
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(new ApiResponse<List<User>>
        {
            Success = true,
            Data = users,
            Message = users.Count == 0 ? "No users available" : null
        });
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetUserById(int Id)
    {
        var user = await _userService.GetUserByIdAsync(Id);

        if (user == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            });
        }

        return Ok(new ApiResponse<User>
        {
            Success = true,
            Data = user
        });
    }
}