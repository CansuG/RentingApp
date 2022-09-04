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
    public async Task<ActionResult<Advert>> Create(AdvertCreate advertCreate)
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

    [HttpGet("filter/{city}")]
    public async Task<ActionResult<List<Advert>>> GetByCity(string city)
    {
        var adverts = await _advertRepository.GetByCityAsync(city);

        return Ok(adverts);
    }

    [HttpGet("filter/{city}/{district}")]
    public async Task<ActionResult<List<Advert>>> GetByDistrict(string city, string district)
    {
        var adverts = await _advertRepository.GetByDistrictAsync(city, district);

        return Ok(adverts);
    }

    [HttpGet("filter/{city}/{district}/{neighbourhood}")]
    public async Task<ActionResult<List<Advert>>> GetByNeighbourhood(string city, string district, string neighbourhood)
    {
        var adverts = await _advertRepository.GetByNeighbourhoodAsync(city, district, neighbourhood);

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
