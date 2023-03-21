using Microsoft.AspNetCore.Mvc;
using Renting.Models.SavedAdvert;
using Renting.Repository;

[Route("api/[controller]")]
[ApiController]
public class SavedAdvertController : ControllerBase
{
    private readonly SavedAdvertRepository _savedAdvertRepository;
    public SavedAdvertController(SavedAdvertRepository savedAdvertRepository)
    {
        _savedAdvertRepository = savedAdvertRepository;
    }

    [HttpPost]
    public async Task<IActionResult> SaveAdvert(SavedAdvertCreate savedAdvertCreate, int applicationUserId, int advertId)
    {
        var savedAdvert = await _savedAdvertRepository.SaveAdvertAsync(savedAdvertCreate, applicationUserId, advertId);
        if (savedAdvert == null)
        {
            return BadRequest("This advert cannot be saved.");
        }
        return Ok("Advert has been saved." + savedAdvert);
    }

    [HttpGet("{applicationUserId}")]
    public async Task<IActionResult> GetSavedAdvert(int applicationUserId)
    {
        var savedAdvert = await _savedAdvertRepository.GetSavedAdvertAsync(applicationUserId);
        if (savedAdvert == null)
        {
            return NotFound("The advert(s) cannot be found.");
        }
        return Ok(savedAdvert);
    }

    [HttpDelete("{savedAdvertId}")]
    public async Task<IActionResult> UnsaveAdvert(int savedAdvertId)
    {
        var affectedRows = await _savedAdvertRepository.UnsaveAdvertAsync(savedAdvertId);
        if (affectedRows > 0)
        {
            return Ok("The advert has been unsaved");
        }
        return NotFound("The advert cannot be found.");
    }
}