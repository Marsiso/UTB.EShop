using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UTB.EShop.Application.DataTransferObjects.Identity;
using UTB.EShop.Application.Interfaces.Identity;

namespace UTB.EShop.Infrastructure.Identity;

public sealed class AuthenticationManager : IAuthenticationManager
{
    /// <summary>
    /// Provides the FindByNameAsync method to find the user by user name and
    /// the CheckPasswordAsync to verify the user’s password against the hashed password from the database.
    /// </summary>
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private User _user;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="configuration"></param>
    /// <param name="user"></param>
    public AuthenticationManager(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Checks whether the user exists in the database and if the password matches.
    /// </summary>
    /// <param name="userForAuth"></param>
    /// <returns></returns>
    public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        _user = await _userManager.FindByNameAsync(userForAuth.UserName);
        return (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password));

    }

    /// <summary>
    /// Creates a token. It does that by collecting information from the private methods and
    /// serializing token options with the <see cref="JwtSecurityTokenHandler"/> WriteToken method.
    /// </summary>
    /// <returns></returns>
    public async Task<string> CreateToken()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
    
    /// <summary>
    /// Returns our secret key as a byte array with the security algorithm.
    /// </summary>
    /// <returns></returns>
    private SigningCredentials GetSigningCredentials()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key =  Encoding.UTF8.GetBytes(jwtSettings.GetSection("serverSecret").Value);
        var secret = new SymmetricSecurityKey(key);
        
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    
    /// <summary>
    /// Creates a list of claims with the user name inside and all the roles the user belongs to.
    /// </summary>
    /// <returns></returns>
    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, _user.UserName)
        };
        
        var roles = await _userManager.GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        return claims;
    }
    
    /// <summary>
    /// Creates an object of the JwtSecurityToken type with all of the required options. 
    /// </summary>
    /// <param name="signingCredentials"></param>
    /// <param name="claims"></param>
    /// <returns></returns>
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken
        (
            issuer: jwtSettings.GetSection("validIssuer").Value,
            audience: jwtSettings.GetSection("validAudience").Value,
            claims: claims,
            expires: 
            DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("expires").Value)),
            signingCredentials: signingCredentials
        );
        
        return tokenOptions;
    }

}