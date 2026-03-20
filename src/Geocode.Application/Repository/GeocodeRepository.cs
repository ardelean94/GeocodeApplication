using Amazon.DynamoDBv2.DataModel;
using Geocode.Application.Abstractions;
using Geocode.Application.Models;
using Geocode.Application.ResultErrors;
using Microsoft.Extensions.Logging;

namespace Geocode.Application.Repository;

public class GeocodeRepository : IGeocodeRepository
{
    private readonly IDynamoDBContext context;
    private readonly ILogger<GeocodeRepository> logger;

    public GeocodeRepository(IDynamoDBContext context, ILogger<GeocodeRepository> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new record in the database
    /// </summary>
    /// <param name="geocode"></param>
    /// <param name="token"></param>
    /// <returns>Returns a success result task with bool true, or a failure if any exception occurs.</returns>
    public async Task<Result<bool>> Create(GeocodeCache geocode, CancellationToken token = default)
    {
        try
        {
            await context.SaveAsync(geocode, token);
        }
        catch(Exception ex)
        {
            logger.LogError("Failed to create record: {Message}", ex.Message);
            return Result.Failure<bool>(GeocodeRepositoryErrors.FailCreateRecord);
        }
        return Result.Success(true);
    }

    /// <summary>
    /// Gets the item from the database that matches the <paramref name="cacheKey"/>
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="token"></param>
    /// <returns>Returns a success result task for both item found or not, or a failure if any exception occurs.</returns>
    public async Task<Result<GeocodeCache?>> Get(string cacheKey, CancellationToken token = default)
    {
        GeocodeCache geocode;
        try
        {
            geocode = await context.LoadAsync<GeocodeCache>(cacheKey, token);
        }
        catch(Exception ex)
        {
            logger.LogError("Failed to load data: {Message}", ex.Message);
            return Result.Failure<GeocodeCache?>(GeocodeRepositoryErrors.FailLoadData);
        }
        
        return Result.Success<GeocodeCache?>(geocode);
    }
}
