using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<IdentityUser> userManager;
    private readonly ITokenRepository tokenRepository;

    public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
    {
      UserManager = userManager;
      this.tokenRepository = tokenRepository;
    }

    public UserManager<IdentityUser> UserManager { get; }

    //POST: /api/Auth/Register
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDto)
    {
      var identityUser = new IdentityUser
      {
        UserName = registerRequestDto.Username,
        Email = registerRequestDto.Username
      };

      var identityResult = await UserManager.CreateAsync(identityUser, registerRequestDto.Password);

      if (identityResult.Succeeded)
      {
        if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
        {
          identityResult = await UserManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

          if (identityResult.Succeeded)
          {
            return Ok("User was registered! Please login.");
          }
          return BadRequest(identityResult.Errors);
        }
        return Ok("User was registered! Please login.");
      }
      else
      {
        return BadRequest(identityResult.Errors);
      }
    }


    //POST: /api/Auth/login
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDto)
    {
      var user = await UserManager.FindByEmailAsync(loginRequestDto.Username);

      if (user != null)
      {
        var checkPasswordResult = await UserManager.CheckPasswordAsync(user, loginRequestDto.Password);

        if (checkPasswordResult)
        {
          //get the roles for the user
          var roles = await UserManager.GetRolesAsync(user);

          if (roles != null)
          {
            //Create Token
            var jwtToken = tokenRepository.CreateJWToken(user, roles.ToList());

            var response = new LoginResponseDto
            {
              JwtToken = jwtToken
            };
            return Ok(response);
          }



        }
      }
      return BadRequest("Username or password incorrect.");
    }
  }
}
