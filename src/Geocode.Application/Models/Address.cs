namespace Geocode.Application.Models;

public class Address
{
    public int HouseNumber { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string StateCode { get; set; }
    public int Zipcode { get; set; }
    public string Country { get; set; }
}
