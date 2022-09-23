using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Renting.Models.Advert;
using Renting.Repository;
using System.IdentityModel.Tokens.Jwt;

namespace Renting.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdvertController : ControllerBase
{
    private readonly IAdvertRepository _advertRepository;

    public AdvertController(IAdvertRepository advertRepository)
    {
        _advertRepository = advertRepository;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Advert>> Create([FromBody]AdvertCreate advertCreate)
    {
        int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

        var advert = await _advertRepository.UpsertAsync(advertCreate, applicationUserId);

        return Ok(advert);
    }

    [HttpGet("{advertId}")]
    public async Task<ActionResult<Advert>> Get(int advertId)
    {
        var advert = await _advertRepository.GetAsync(advertId);

        return Ok(advert);
    }

    [HttpGet("user/{applicationUserId}")]
    public async Task<ActionResult<List<Advert>>> GetByApplicationUserId(int applicationUserId)
    {
        var adverts = await _advertRepository.GetAllByUserIdAsync(applicationUserId);

        return Ok(adverts);
    }

    [HttpPost("filter")]
    public async Task<ActionResult<List<Advert>>> GetAdvertsWithFilters([FromBody]Filtering filter)
    {
        if(!filter.District.Equals("")  && filter.City.Equals(""))
        {
            return BadRequest("You cannot select District, and Neighbourhood without selecting City.");
        }
        else if ((!filter.Neighbourhood.Equals("") && filter.District.Equals("") && filter.City.Equals("")))
        {
            return BadRequest("You cannot select Neighbourhood without selecting City, and District.");
        }
        else if( filter.City.Equals("") && (!filter.Rooms.Equals("") || !filter.MaxPrice.Equals("") || 
            !filter.MinPrice.Equals("") || !filter.MaxFloorArea.Equals("") || !filter.MinFloorArea.Equals("") ||
            !filter.OrderByWith.Equals("")) )
        {
            return BadRequest("You cannot select this field before you select City.");
        }

        var adverts = await _advertRepository.GetAdvertsWithFiltersAsync(filter);

        return Ok(adverts);
    }


    [Authorize]
    [HttpDelete("{advertId}")]
    public async Task<ActionResult<int>> Delete(int advertId)
    {
        int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

        var foundAdvert = await _advertRepository.GetAsync(advertId);

        if (foundAdvert == null) return BadRequest("Advert does not exist");

        if (foundAdvert.ApplicationUserId == applicationUserId)
        {
            var affectedRows = await _advertRepository.DeleteAsync(advertId);

            return Ok(affectedRows);
        }
        else
        {
            return BadRequest("You did not create this advert.");
        }

    }
}
