using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Renting.Models.Account;
using Renting.Repository;
using Renting.Services;

namespace Renting.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUserIdentity> _userManager;
    private readonly SignInManager<ApplicationUserIdentity> _signInManager;
    private readonly IAccountService _accountService;
    private readonly IAccountRepository _accountRepository;

    public AccountController(
        ITokenService tokenService,
        UserManager<ApplicationUserIdentity> userManager,
        SignInManager<ApplicationUserIdentity> signInManager,
        IAccountService accountService,
        IAccountRepository accountRepository)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
        _accountRepository = accountRepository;
    }

    [HttpPut]
    public async Task<ActionResult<GetAccount>> Update(GetAccount userInfo)
    {
        /*var usernames = await _accountRepository.GetUsernamesAsync();
        var resultUsername = _accountService.UniqueForUsername(userInfo.Username, usernames);

        var emails = await _accountRepository.GetEmailsAsync();
        var resultEmail = _accountService.UniqueForEmail(userInfo.Email, emails);

        if (!resultUsername.Equals("ok"))
        {
            return BadRequest(resultUsername);
        }

        if (!resultEmail.Equals("ok"))
        {
            return BadRequest(resultEmail);
        }
        */

        ApplicationUserIdentity identity = new ApplicationUserIdentity()
        {
            ApplicationUserId = userInfo.ApplicationUserId,
            Username = userInfo.Username,
            Email = userInfo.Email,
            Gender = userInfo.Gender,
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName
        };

        var result = await _userManager.UpdateAsync(identity);

        if(result.Succeeded)
        {
            identity = await _userManager.FindByIdAsync(userInfo.ApplicationUserId.ToString());

            GetAccount account = new GetAccount()
            {
                ApplicationUserId = identity.ApplicationUserId,
                Username = identity.Username,
                Email=identity.Email,
                Gender = identity.Gender,
                FirstName = identity.FirstName,
                LastName = identity.LastName
            };
            return account;
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApplicationUser>> Register(ApplicationUserCreate applicationUserCreate)
    {
        /*var usernames = await _accountRepository.GetUsernamesAsync();
        var resultUsername = _accountService.UniqueForUsername(applicationUserCreate.Username, usernames);

        var emails = await _accountRepository.GetEmailsAsync();
        var resultEmail = _accountService.UniqueForEmail(applicationUserCreate.Email, emails);

        if (!resultUsername.Equals("ok"))
        {
            return BadRequest(resultUsername);
        }

        if (!resultEmail.Equals("ok"))
        {
            return BadRequest(resultEmail);
        }*/

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

    [HttpGet("{applicationUserId}")]
    public async Task<ActionResult<ApplicationUserIdentity>> Get(int applicationUserId)
    {
        var account = await _userManager.FindByIdAsync(applicationUserId.ToString());

        GetAccount accountInfo = new GetAccount()
        { 
            ApplicationUserId = account.ApplicationUserId,
            Username = account.Username,
            Email = account.Email,
            FirstName = account.FirstName,
            LastName = account.LastName,
            Gender = account.Gender,
        };

        return Ok(accountInfo);
    }

    /*[HttpGet("control/{username}")]
    public async Task<ActionResult<string>> ControlUniqueForUsername(string username)
    {
        var usernames = await _accountRepository.GetUsernamesAsync();

        var result = _accountService.UniqueForUsername(username, usernames);

        if (!result.Equals("ok"))
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("control/{email}")]
    public async Task<ActionResult<string>> ControlUniqueForEmail(string email)
    {
        var emails = await _accountRepository.GetEmailsAsync();

        var result = _accountService.UniqueForEmail(email, emails);

        if (!result.Equals("ok"))
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
    */
}
