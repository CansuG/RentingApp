using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Renting.Models.Photo;
using Renting.Repository;
using Renting.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Renting.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhotoController : ControllerBase
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IAdvertRepository _blogRepository;
    private readonly IPhotoService _photoService;

    public PhotoController(
        IPhotoRepository photoRepository,
        IAdvertRepository blogRepository,
        IPhotoService photoService)
    {
        _photoRepository = photoRepository;
        _blogRepository = blogRepository;
        _photoService = photoService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Photo>> UploadPhoto(IFormFile file)
    {
        int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

        var uploadResult = await _photoService.AddPhotosAsync(file);

        if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);

        var photoCreate = new PhotoCreate
        {
            PublicId = uploadResult.PublicId,
            ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
            Description = file.FileName
        };

        var photo = await _photoRepository.InsertAsync(photoCreate, applicationUserId);

        return Ok(photo);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<Photo>>> GetByApplicationUserId()
    {
        int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

        var photos = await _photoRepository.GetAllByUserIdAsync(applicationUserId);

        return Ok(photos);

    }

    [HttpGet("{photoId}")]
    public async Task<ActionResult<Photo>> Get(int photoId)
    {
        var photo = await _photoRepository.GetAsync(photoId);

        return Ok(photo);
    }


}
