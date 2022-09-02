using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Renting.Models.Account;
using Renting.Services;

namespace Renting.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUserIdentity> _userManager;
    private readonly SignInManager<ApplicationUserIdentity> _signInManager;

    public AccountController(
        ITokenService tokenService,
        UserManager<ApplicationUserIdentity> userManager,
        SignInManager<ApplicationUserIdentity> signInManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApplicationUser>> Register(ApplicationUserCreate applicationUserCreate)
    {

        var applicationUserIdentity = new ApplicationUserIdentity
        {
            Username = applicationUserCreate.Username,
            Email = applicationUserCreate.Email,
            Gender = applicationUserCreate.Gender,
            FirstName = applicationUserCreate.FirstName,
            LastName = applicationUserCreate.LastName,
        };

        var result = await _userManager.CreateAsync(applicationUserIdentity, applicationUserCreate.Password);

        if (result.Succeeded)
        {
            applicationUserIdentity = await _userManager.FindByNameAsync(applicationUserCreate.Username);

            ApplicationUser user = new ApplicationUser()
            {
                ApplicationUserId = applicationUserIdentity.ApplicationUserId,
                Username = applicationUserIdentity.Username,
                FirstName = applicationUserIdentity.FirstName,
                LastName = applicationUserIdentity.LastName,
                Email = applicationUserIdentity.Email,
                Gender = applicationUserIdentity.Gender,
                Token = _tokenService.CreateToken(applicationUserIdentity)
            };

            return user;
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApplicationUser>> Login(ApplicationUserLogin applicationUserLogin)
    {
        var applicationUserIdentity = await _userManager.FindByEmailAsync(applicationUserLogin.Email);

        if (applicationUserIdentity != null)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(
                applicationUserIdentity,
                applicationUserLogin.Password,
                false);

            if (result.Succeeded)
            {
                ApplicationUser applicationUser = new ApplicationUser()
                {
                    ApplicationUserId = applicationUserIdentity.ApplicationUserId,
                    Username = applicationUserIdentity.Username,
                    Email = applicationUserIdentity.Email,
                    FirstName = applicationUserIdentity.FirstName,
                    LastName = applicationUserIdentity.LastName,
                    Gender = applicationUserIdentity.Gender,
                    Token = _tokenService.CreateToken(applicationUserIdentity)
                };

                return Ok(applicationUser);
            }

        }

        return BadRequest("Invalid login attempt.");
    }
}
