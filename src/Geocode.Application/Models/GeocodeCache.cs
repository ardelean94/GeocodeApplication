using Amazon.DynamoDBv2.DataModel;

namespace Geocode.Application.Models;

[DynamoDBTable("geocodes")]
public class GeocodeCache
{
    [DynamoDBHashKey("addresskey")]
    public required string AddressCacheKey { get; init; }
    public required string NormalizedAddress { get; init; }
    public required string OriginalAddress { get; init; }
    public required int HouseNumber { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string StateCode { get; init; }
    public required int Zipcode { get; init; }
    public required string Country { get; init; }
    public required string GoogleResponse { get; init; }
    public required string CreatedAt { get; init; }
    
    [DynamoDBProperty]
    public required long ExpiresAt { get; init; }
}
