using Microsoft.AspNetCore.Mvc;
using Geocode.Contracts.Requests;
using Geocode.Application.Services;
using System.Threading.Tasks;
using Geocode.API.Mapping;

namespace Geocode.API.Controllers;

[ApiController]
[Route("geocode")]
public class GeocodeController : ControllerBase
{
    private readonly IGeocodeService geocodeService;

    public GeocodeController(IGeocodeService geocodeService)
    {
        this.geocodeService = geocodeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] AddressRequest request, CancellationToken token)
    {
        var response = await geocodeService.Get(request.Address, token);

        if (response is null) return NotFound();

        return Ok(response.MapToResponse());
    }
}