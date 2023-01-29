using Microsoft.AspNetCore.Mvc;
using Renting.Models.AdvertPhoto;
using Renting.Models.Photo;
using Renting.Repository;
using Renting.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Renting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertPhotoController : ControllerBase
    {
        private readonly IAdvertPhotoRepository _advertPhotoRepository;
        private readonly IPhotoService _photoService;

        public AdvertPhotoController(IAdvertPhotoRepository advertPhotoRepository, IPhotoService photoService)
        {
            _advertPhotoRepository = advertPhotoRepository;
            _photoService = photoService;
        }

        [HttpGet("{advertId}")]
        public async Task<IActionResult> GetPhotosByAdvertId(int advertId)
        {
            var photos = await _advertPhotoRepository.GetPhotoByAdvertIdAsync(advertId);
            if(photos.Count == 0)
            {
                return NotFound("There is no photo for this advert.");
            }
            return Ok(photos);
        }

        [HttpGet("photo/{photoId}")]
        public async Task<IActionResult> GetPhotoByPhotoId(int photoId)
        {
            var photo = await _advertPhotoRepository.GetPhotoByPhotoIdAsync(photoId);
            if (photo == null)
            {
                return NotFound("The photo was not found");
            }
            return Ok(photo);
        }

        [HttpPost("upload")]
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

            return Ok(photoCreate);
        }

        [HttpPost]
        public async Task<ActionResult<AdvertPhoto>> AddPhoto(AdvertPhoto advertPhoto)
        {
            AdvertPhotoCreate advertPhotoCreate = new AdvertPhotoCreate
            {
                PublicId = advertPhoto.PublicId,
                ImageUrl = advertPhoto.ImageUrl
            };

            var newPhoto = await _advertPhotoRepository.AddPhotoAsync(advertPhotoCreate, advertPhoto.AdvertId);

            if (newPhoto == null)
            {
                return BadRequest("You cannot add more than 10 photos");
            }

            return Ok(newPhoto);
        }

        [HttpDelete("delete/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var foundPhoto = await _advertPhotoRepository.GetPhotoByPhotoIdAsync(photoId);
            if(foundPhoto == null) { return NotFound("Photo did not found!"); }

            var deleteResult = await _photoService.DeletePhotoAsync(foundPhoto.PublicId);
            if (deleteResult.Error != null) return BadRequest(deleteResult.Error.Message);

            var affectedRows = await _advertPhotoRepository.DeletePhotoAsync(photoId);
            if (affectedRows > 0)
            {
                return Ok("The photo has been deleted");
            }
            return NotFound("The photo was not found");
        }
    }
}

