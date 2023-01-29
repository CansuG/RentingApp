using Microsoft.AspNetCore.Mvc;
using Renting.Models.AdvertPhoto;
using Renting.Repository;
using System.Threading.Tasks;

namespace Renting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertPhotoController : ControllerBase
    {
        private readonly IAdvertPhotoRepository _advertPhotoRepository;

        public AdvertPhotoController(IAdvertPhotoRepository advertPhotoRepository)
        {
            _advertPhotoRepository = advertPhotoRepository;
        }

        [HttpGet("{advertId}")]
        public async Task<IActionResult> GetPhotosByAdvertId(int advertId)
        {
            var photos = await _advertPhotoRepository.GetPhotoByAdvertIdAsync(advertId);
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

        [HttpPost]
        public async Task<IActionResult> AddPhoto(AdvertPhotoCreate advertPhotoCreate, int advertId)
        {
            var newPhoto = await _advertPhotoRepository.AddPhotoAsync(advertPhotoCreate, advertId);
            if (newPhoto == null)
            {
                return BadRequest("You cannot add more than 10 photos");
            }
            return Ok(newPhoto);
        }

        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var affectedRows = await _advertPhotoRepository.DeletePhotoAsync(photoId);
            if (affectedRows > 0)
            {
                return Ok("The photo has been deleted");
            }
            return NotFound("The photo was not found");
        }
    }
}

