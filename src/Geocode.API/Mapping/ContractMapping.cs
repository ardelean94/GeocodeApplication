using Amazon.Runtime.Internal;
using Geocode.Application.Models;
using Geocode.Contracts.Requests;
using System.Collections.Specialized;

namespace Geocode.API.Mapping;

public static class ContractMapping
{
    public static Address MapToAddress(this AddressRequest request)
    {
        var address = ProcessAddressPattern(request.Address);

        return address;
    }

    private static Address ProcessAddressPattern(string address)
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
        var addressParts = address.Split(',');
        if (addressParts.Length != addressComponentNumber)
            throw new ArgumentException($"The address must follow the US address pattern: {usAddressPattern}");

        var houseAndStreet = addressParts[0].Split(' ', 2);
        if (houseAndStreet.Length != 2)
            throw new ArgumentException($"The house or street are invalid, please follow the US address pattern: {usAddressPattern}");

        var stateAndZipcode = addressParts[2].Split(' ');
        if (stateAndZipcode.Length != 2)
            throw new ArgumentException($"The state or zip code are invalid, please follow the US address pattern: {usAddressPattern}");

        var houseNumber = houseAndStreet[0];
        var street = houseAndStreet[1];
        var city = addressParts[1];
        var stateCode = stateAndZipcode[0];
        var zipcode = stateAndZipcode[1];
        var country = addressParts[3];

        return new Address
        {
            HouseNumber = int.Parse(houseNumber),
            Street = street,
            City = city,
            StateCode = stateCode,
            Zipcode = int.Parse(zipcode),
            Country = country
        };
    }
}
