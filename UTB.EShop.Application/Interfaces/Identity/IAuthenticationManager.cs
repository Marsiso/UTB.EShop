using UTB.EShop.Application.DataTransferObjects.Identity;

namespace UTB.EShop.Application.Interfaces.Identity;

public interface IAuthenticationManager
{
    Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
    Task<string> CreateToken();
}