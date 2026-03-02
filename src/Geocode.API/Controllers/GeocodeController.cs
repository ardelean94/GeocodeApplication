using Microsoft.AspNetCore.Mvc;
using Geocode.Contracts;
using Geocode.Contracts.Requests;
using Amazon.Lambda.Core;
using Geocode.Application.Services;
using System.Threading.Tasks;
using Geocode.Application.Models;
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
        Address address = request.MapToAddress();

        var response = await geocodeService.Get(address, token);

        return Ok(response);
    }
}