namespace Geocode.Application.Models;

public class Address
{
    public required string InputAddress { get; init; }
    public required int HouseNumber { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string StateCode { get; init; }
    public required int Zipcode { get; init; }
    public required string Country { get; init; }
}
