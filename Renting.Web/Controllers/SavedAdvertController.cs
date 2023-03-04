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
        return Ok(savedAdvert);
    }

    [HttpGet("{savedAdvertId}")]
    public async Task<IActionResult> GetSavedAdvert(int savedAdvertId)
    {
        var savedAdvert = await _savedAdvertRepository.GetSavedAdvertAsync(savedAdvertId);
        if (savedAdvert == null)
        {
            return NotFound("The advert cannot be found.");
        }
        return Ok(savedAdvert);
    }

    [HttpGet("{applicationUserId}")]
    public async Task<IActionResult> GetSavedAdverts(int applicationUserId)
    {
        var savedAdverts = await _savedAdvertRepository.GetSavedAdvertsAsync(applicationUserId);
        if (savedAdverts.Count == 0)
        {
            return NotFound("The advert(s) cannot be found.");
        }
        return Ok(savedAdverts);
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