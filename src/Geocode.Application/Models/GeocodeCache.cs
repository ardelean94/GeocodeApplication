using Amazon.DynamoDBv2.DataModel;

namespace Geocode.Application.Models;

[DynamoDBTable("geocodes")]
public class GeocodeCache
{
    [DynamoDBHashKey("addresskey")]
    public required string AddressCacheKey { get; init; }

    [DynamoDBProperty("normalizedaddress")]
    public required string NormalizedAddress { get; init; }

    [DynamoDBProperty("originaladdress")]
    public required string OriginalAddress { get; init; }

    [DynamoDBProperty("housenumber")]
    public required int HouseNumber { get; init; }

    [DynamoDBProperty("street")]
    public required string Street { get; init; }

    [DynamoDBProperty("city")]
    public required string City { get; init; }

    [DynamoDBProperty("statecode")]
    public required string StateCode { get; init; }

    [DynamoDBProperty("zipcode")]
    public required int Zipcode { get; init; }

    [DynamoDBProperty("country")]
    public required string Country { get; init; }

    [DynamoDBProperty("googleresponse")]
    public required GoogleGeocodeModel GoogleResponse { get; init; }

    [DynamoDBProperty("createdat")]
    public required string CreatedAt { get; init; }
    
    [DynamoDBProperty("expiresat")]
    public required long ExpiresAt { get; init; }
}
