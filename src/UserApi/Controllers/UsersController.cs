using Microsoft.AspNetCore.Mvc;
using UserApi.Services;
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
        return Ok(users);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetUserById(int Id)
    {
        var user = await _userService.GetUserByIdAsync(Id);
        if (user == null || user.Id == 0)
        {
            return NotFound();
        }
        return Ok(user);
    }
}