using Microsoft.AspNetCore.Identity;

namespace UTB.EShop.Infrastructure.Identity;

public sealed class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}