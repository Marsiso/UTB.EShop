using System.ComponentModel.DataAnnotations;

namespace UTB.EShop.Application.DataTransferObjects.Identity;

public sealed class UserForAuthenticationDto
{
    [Required(ErrorMessage = "User name is required")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Password name is required")]
    public string Password { get; set; } = null!;
}