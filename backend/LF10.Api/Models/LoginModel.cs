namespace LF10.Api.Models;

public class LoginModel
{
    [Required, MinLength(3)]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
