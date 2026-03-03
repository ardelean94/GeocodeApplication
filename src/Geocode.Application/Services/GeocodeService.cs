using Geocode.Application.Models;
using Geocode.Application.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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
        var geocodeRequest = Path.Combine(configuration["GOOGLE_API_GET_GEOCODE"], configuration["GOOGLE_API_GEOCODE_REQUEST_FORMAT"]);
        httpClient.BaseAddress = new Uri(geocodeRequest);
        httpClient.Timeout = TimeSpan.FromSeconds(5);

        apiKey = configuration["GOOGLE_GEOCODE_API_KEY"];
    }

    public async Task<GoogleGeocodeModel> Get(string address, CancellationToken token = default)
    {
        GoogleGeocodeModel result = null;
        string normalizedAddress = NormalizeAddress(address);
        string addressCacheKey = BuildCacheKey(normalizedAddress);
        result = await geocodeRepository.Get(addressCacheKey, token);

        if (result is null)
        {
            var googleGeocode = await FetchGeocodeDataAsync(address, token);
            GeocodeCache geocodeCache = BuildGeocodeCache(address, googleGeocode);

            await Create(geocodeCache, token);
            return googleGeocode;
        }

        return result;
    }

    public async Task Create(GeocodeCache geocode, CancellationToken token = default)
    {
        await geocodeRepository.Create(geocode, token);
    }

    private GeocodeCache BuildGeocodeCache(string address, GoogleGeocodeModel model)
    {
        //  US address pattern:
        //  {house_number} {street}, {city}, {state_code} {zip_code}, {country}
        const int addressComponentNumber = 4;
        string usAddressPattern = "{house_number} {street}, {city}, {state_code} {zip_code}, {country}";

        /* 
         * Address after split for `,`
            addressPattern[0] {house_number} {street}
            addressPattern[1] {city}
            addressPattern[2] {state_code} {zip_code}
            addressPattern[3] {country}
        */
        var addressParts = address.Trim().Split(',');
        if (addressParts.Length != addressComponentNumber)
            throw new ArgumentException($"The address must follow the US address pattern: {usAddressPattern}");

        var houseAndStreet = addressParts[0].Trim().Split(' ', 2);
        if (houseAndStreet.Length != 2)
            throw new ArgumentException($"The house or street are invalid, please follow the US address pattern: {usAddressPattern}");

        var stateAndZipcode = addressParts[2].Trim().Split(' ');
        if (stateAndZipcode.Length != 2)
            throw new ArgumentException($"The state or zip code are invalid, please follow the US address pattern: {usAddressPattern}");

        var houseNumber = houseAndStreet[0];
        var street = houseAndStreet[1];
        var city = addressParts[1];
        var stateCode = stateAndZipcode[0];
        var zipcode = stateAndZipcode[1];
        var country = addressParts[3];
        var normalizedAddress = NormalizeAddress(address);
        var cacheKey = BuildCacheKey(normalizedAddress);

        return new GeocodeCache
        {
            AddressCacheKey = cacheKey,
            NormalizedAddress = normalizedAddress,
            OriginalAddress = address,
            HouseNumber = int.Parse(houseNumber),
            Street = street,
            City = city,
            StateCode = stateCode,
            Zipcode = int.Parse(zipcode),
            Country = country,
            GoogleResponse = JsonSerializer.Serialize(model),
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds()
        };
    }

    private string BuildCacheKey(string normalizedAddress)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedAddress));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private string NormalizeAddress(string address)
    {
        return address
            .Trim()
            .ToLowerInvariant()
            .Replace("  ", " ")
            .Replace("+", " ")
            .Replace("%2C", " ")
            .Replace("united states", "us")
            .Replace("u.s.a", "us")
            .Replace("usa", "us");
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
