using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Users;

public class LoginUserDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}