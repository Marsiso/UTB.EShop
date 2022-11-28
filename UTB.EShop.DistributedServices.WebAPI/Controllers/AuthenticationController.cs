using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UTB.EShop.Application.DataTransferObjects.Identity;
using UTB.EShop.Application.Interfaces.Identity;
using UTB.EShop.DistributedServices.WebAPI.Attributes;
using UTB.EShop.Infrastructure.Identity;
using ILogger = Serilog.ILogger;

namespace UTB.EShop.DistributedServices.WebAPI.Controllers;

[Route("api/[Controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IAuthenticationManager _authManager;
    
    public AuthenticationController (ILogger logger, IMapper mapper, UserManager<User> userManager, IAuthenticationManager authManager)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _authManager = authManager;
    }
    
    [HttpPost(Name = "RegisterUser")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
    {
        var user = _mapper.Map<User>(userForRegistration);
        
        var result = await _userManager.CreateAsync(user, userForRegistration.Password);
        if(!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            
            return BadRequest(ModelState);
        }
        
        await _userManager.AddToRolesAsync(user, userForRegistration.Roles);
        
        return StatusCode(201);
    }
    
    [HttpPost("login", Name = "AuthenticateUser")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
    {
        if (!await _authManager.ValidateUser(user))
        {
            _logger.Warning($"{nameof(Authenticate)}: Authentication failed. Wrong user name or password.");
            return Unauthorized();
        }
        
        return Ok(new { Token = await _authManager.CreateToken() });
    }

}