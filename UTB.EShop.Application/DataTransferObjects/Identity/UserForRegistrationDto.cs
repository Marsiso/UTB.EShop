using System.ComponentModel.DataAnnotations;

namespace UTB.EShop.Application.DataTransferObjects.Identity;

public sealed class UserForRegistrationDto
{
    [StringLength(50)]
    public string FirstName { get; set; } = null!;
    
    [StringLength(50)]
    public string LastName { get; set; } = null!;
    
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
    
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Phone]
    public string PhoneNumber { get; set; } = null!;
    
    public ICollection<string>? Roles { get; set; } = null!;
}