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
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginationRequest request)
    {
        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            request.PageNumber = Math.Max(request.PageNumber, 1);
            request.PageSize = Math.Clamp(request.PageSize, 1, 100);
        }   
        var users = await _userService.GetAllUsersAsync(request.PageNumber,request.PageSize);

        return Ok(users);
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