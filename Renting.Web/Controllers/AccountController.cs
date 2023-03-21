using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Renting.Models.Account;
using Renting.Models.Photo;
using Renting.Repository;
using Renting.Services;
using System.IdentityModel.Tokens.Jwt;

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
    private readonly IPhotoService _photoService;

    public AccountController(
        ITokenService tokenService,
        UserManager<ApplicationUserIdentity> userManager,
        SignInManager<ApplicationUserIdentity> signInManager,
        IAccountService accountService,
        IAccountRepository accountRepository,
        IPhotoService photoService)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
        _accountRepository = accountRepository;
        _photoService = photoService;

    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<GetAccount>> Update(GetAccount userInfo)
    {
        int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);
        var user = await _userManager.FindByIdAsync(applicationUserId.ToString());

        var usernames = await _accountRepository.GetUsernamesAsync();
        var resultUsername = _accountService.UniqueForYourUsername(userInfo.Username,user.Username, usernames);

        if (!resultUsername.Equals("ok"))
        {
            return BadRequest(resultUsername);
        }

        ApplicationUserIdentity identity = new ApplicationUserIdentity()
        {
            ApplicationUserId = userInfo.ApplicationUserId,
            Username = userInfo.Username,
            Email = userInfo.Email,
            Gender = userInfo.Gender,
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            PublicId = userInfo.PublicId,
            ImageUrl = userInfo.ImageUrl,
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
                LastName = identity.LastName,
                PublicId = identity.PublicId,
                ImageUrl = identity.ImageUrl,
            };
            return Ok("User's information is updated." + account);
        }

        return BadRequest(result);
    }

    [Authorize]
    [HttpPut("profile_photo")]
    public async Task<ActionResult<GetAccount>> UpdateProfilePhoto(GetAccount userInfo)
    {
        var resultUpdate = await _accountRepository.UpdateProfilePhotoAsync(userInfo.ApplicationUserId, userInfo.PublicId, userInfo.ImageUrl);

        if (resultUpdate.Succeeded)
        {
            var identity = await _userManager.FindByIdAsync(userInfo.ApplicationUserId.ToString());

            GetAccount account = new GetAccount()
            {
                ApplicationUserId = identity.ApplicationUserId,
                Username = identity.Username,
                Email = identity.Email,
                Gender = identity.Gender,
                FirstName = identity.FirstName,
                LastName = identity.LastName,
                PublicId = identity.PublicId,
                ImageUrl = identity.ImageUrl,
            };
            return Ok("User's profile photo is updated." + account); ;
        }

        return BadRequest(resultUpdate);

    }

    [HttpPost("register")]
    public async Task<ActionResult<ApplicationUser>> Register(ApplicationUserCreate applicationUserCreate)
    {
        var usernames = await _accountRepository.GetUsernamesAsync();
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
        }

        var applicationUserIdentity = new ApplicationUserIdentity
        {
            Username = applicationUserCreate.Username,
            Email = applicationUserCreate.Email,
            Gender = applicationUserCreate.Gender,
            FirstName = applicationUserCreate.FirstName,
            LastName = applicationUserCreate.LastName,
            PublicId = applicationUserCreate.PublicId,
            ImageUrl = applicationUserCreate.ImageUrl, 
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
                PublicId=applicationUserIdentity.PublicId,
                ImageUrl = applicationUserIdentity.ImageUrl,
                Token = _tokenService.CreateToken(applicationUserIdentity)
            };

            return Ok("Account is created succesfully." + user);
        }

        return BadRequest(result);
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
                    PublicId = applicationUserIdentity.PublicId,
                    ImageUrl = applicationUserIdentity.ImageUrl,
                    Token = _tokenService.CreateToken(applicationUserIdentity)
                };

                return Ok("Login is succesfull." + applicationUser);
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
            PublicId=account.PublicId,
            ImageUrl = account.ImageUrl,
        };

        return Ok(accountInfo);
    }

    [HttpGet("controlUsername/{username}")]
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
    
    [HttpGet("controlEmail/{email}")]
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

    [Authorize]
    [HttpPost("upload_photo")]
    public async Task<ActionResult<PhotoCreate>> UploadPhoto(IFormFile file)
    {

        var uploadResult = await _photoService.AddPhotosAsync(file);

        if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);

        var photoCreate = new PhotoCreate
        {
            PublicId = uploadResult.PublicId,
            ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
            Description = file.FileName
        };

        return Ok("Photo uploaded to cloudinary." + photoCreate );
    }

    [Authorize]
    [HttpDelete("profile_photo")]
    public async Task<ActionResult<int>> DeletePhoto()
    {
        int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

        var user = await _userManager.FindByIdAsync(applicationUserId.ToString());

        var publicId = user.PublicId;

        var avatarPhotoPublicId = "tyjcpvmmrjjcwplppxfo";
        var avatarPhotoImageUrl = "https://res.cloudinary.com/ddkjxhjyy/image/upload/v1677964732/tyjcpvmmrjjcwplppxfo.png";

        if (publicId.Equals(avatarPhotoPublicId))
        {
            return BadRequest("This photo cannot be deleted."); 
            // Avatar photo shouldn't be deleted.
        }
        var uploadResult = await _photoService.DeletePhotoAsync(publicId);

        if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);

        var resultUpdate = await _accountRepository.UpdateProfilePhotoAsync(applicationUserId, avatarPhotoPublicId, avatarPhotoImageUrl);

        return Ok("Photo deleted.");
    }

}
