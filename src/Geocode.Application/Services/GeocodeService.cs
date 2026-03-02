using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Geocode.Application.Models;
using Geocode.Application.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;

namespace Geocode.Application.Services;

public class GeocodeService : IGeocodeService
{
    private readonly IConfiguration configuration;
    private readonly ILogger<GeocodeService> logger;
    private readonly IGeocodeRepository geocodeRepository;
    private static readonly HttpClient httpClient = new();
    private readonly string apiKey = string.Empty;

    public GeocodeService(IConfiguration configuration, 
        ILogger<GeocodeService> logger,
        IGeocodeRepository geocodeRepository)
	{
        this.configuration = configuration;
        this.logger = logger;
        this.geocodeRepository = geocodeRepository;
        httpClient.BaseAddress = new Uri(configuration["GOOGLE_API_GET_GEOCODE"]);
        httpClient.Timeout = TimeSpan.FromSeconds(5);

        apiKey = configuration["GOOGLE_GEOCODE_API_KEY"];
    }

    public async Task<GoogleGeocodeModel> Get(Address address, CancellationToken token = default)
    {
        var result = await geocodeRepository.Get(address, token);
        return result;
    }

    public async Task Create(GoogleGeocodeModel geocode, CancellationToken token = default)
    {
        await geocodeRepository.Create(geocode, token);
    }

    private async Task<GoogleGeocodeModel?> FetchGeocodeDataAsync(string address, CancellationToken token = default)
    {
        var encodedAddress = Uri.EscapeDataString(address);
        var requestUrl = $"?address={encodedAddress}&key={apiKey}";
        HttpResponseMessage response;
        GoogleGeocodeModel? output = null;

        try
        {
            response = await httpClient.GetAsync(requestUrl, token);
            
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync(token);
                output = await DeserializeResponseAsync(contentStream, token);
            }
        }
        catch(TaskCanceledException ex) when (!token.IsCancellationRequested)
        {
            logger.LogError($"Fetch geocode with data {address} error: {ex.Message}");
            throw;
        }

        return output;
    }

    private async Task<GoogleGeocodeModel?> DeserializeResponseAsync(Stream stream, CancellationToken token)
    {
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        GoogleGeocodeModel? geocode = null;

        try
        {
            geocode = await JsonSerializer.DeserializeAsync<GoogleGeocodeModel>(
                stream,
                jsonOptions,
                token);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to deserialize data", ex);
            throw;
        }
        return geocode;
    }
}
