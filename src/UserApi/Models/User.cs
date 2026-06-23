namespace UserApi.Models;

public class User
{
    public int UserId { get; set; }
    public int ExternalId { get; set; }
    public string Name { get; set; }= string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}