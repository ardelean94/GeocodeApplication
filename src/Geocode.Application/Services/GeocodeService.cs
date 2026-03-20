using Geocode.Application.Abstractions;
using Geocode.Application.Extensions;
using Geocode.Application.Models;
using Geocode.Application.Options;
using Geocode.Application.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Geocode.Application.Services;

public class GeocodeService : IGeocodeService
{
    private readonly ILogger<GeocodeService> logger;
    private readonly IGeocodeRepository geocodeRepository;
    private readonly IGeocodeHttp geocodeHttp;
    private readonly IOptions<DynamoDBOptions> dynamoDbOptions;
    private readonly IOptions<GeocodeOptions> geocodeOptions;
    private readonly int cacheTTLDays;
    private readonly string createdAtDateFormat = string.Empty;

    public GeocodeService(
        ILogger<GeocodeService> logger,
        IGeocodeRepository geocodeRepository,
        IGeocodeHttp geocodeHttp,
        IOptions<DynamoDBOptions> dynamoOptions,
        IOptions<GeocodeOptions> geocodeOptions)
	{
        this.logger = logger;
        this.geocodeRepository = geocodeRepository;
        this.geocodeHttp = geocodeHttp;
        this.dynamoDbOptions = dynamoOptions;
        this.geocodeOptions = geocodeOptions;
    }

    public async Task<Result<GeocodeCache>> Get(string addressInput, CancellationToken token = default)
    {
        var address = addressInput.FormatAddress();

        if (address.IsFailure)
            return Result.Failure<GeocodeCache>(address.Error);

        string normalizedAddress = NormalizeAddress(addressInput);
        string addressCacheKey = BuildCacheKey(normalizedAddress);
        
        var cacheItem = await geocodeRepository.Get(addressCacheKey, token);

        if (cacheItem.IsFailure)
            return Result.Failure<GeocodeCache>(cacheItem.Error);
        
        //  cache hit
        if(cacheItem.Value is not null)
            return Result.Success<GeocodeCache>(cacheItem.Value!);

        //  cache miss
        var googleGeocodeResult = await geocodeHttp.FetchGeocodeDataAsync(addressInput, geocodeOptions.Value.ApiKey, token);
        
        if (googleGeocodeResult.IsFailure)
            return Result.Failure<GeocodeCache>(googleGeocodeResult.Error);

        var geocodeRecord = BuildGeocodeCache(address.Value, addressCacheKey, normalizedAddress, googleGeocodeResult.Value!);

        var result = await geocodeRepository.Create(geocodeRecord, token);
        if (result.IsFailure)
            return Result.Failure<GeocodeCache>(result.Error);

        return Result.Success(geocodeRecord);
    }

    internal GeocodeCache BuildGeocodeCache(Address address, string cacheKey, string normalized, GoogleGeocodeModel model)
    {
        return new GeocodeCache
        {
            AddressCacheKey = cacheKey,
            NormalizedAddress = normalized,
            OriginalAddress = address.InputAddress,
            HouseNumber = address.HouseNumber,
            Street = address.Street,
            City = address.City,
            StateCode = address.StateCode,
            Zipcode = address.Zipcode,
            Country = address.Country,
            GoogleResponse = model,
            CreatedAt = DateTime.UtcNow.ToString(dynamoDbOptions.Value.CreatedAtDateFormat),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(dynamoDbOptions.Value.CacheTTLDays).ToUnixTimeSeconds()
        };
    }

    internal string BuildCacheKey(string normalizedAddress)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedAddress));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    internal string NormalizeAddress(string address)
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
}