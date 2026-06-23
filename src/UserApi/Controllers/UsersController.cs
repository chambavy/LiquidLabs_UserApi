using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{  
    private readonly IUserRepository _userRepository;
    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return Ok(users);
    }
    [HttpGet("{ExternalId}")]
    public async Task<IActionResult> GetUserById(int ExternalId)
    {
        var user = await _userRepository.GetUserById(ExternalId);
        if (user == null || user.ExternalId == 0)
        {
            return NotFound();
        }
        return Ok(user);
    }
}