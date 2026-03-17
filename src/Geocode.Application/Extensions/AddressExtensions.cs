using Geocode.Application.Models;

namespace Geocode.Application.Extensions;

internal static class AddressExtensions
{
    internal static Address FormatAddress(this string address)
    {
        if (string.IsNullOrEmpty(address))
            throw new ArgumentException(nameof(address), "Address cannot be null or empty");

        //  US address pattern:
        //  {house_number} {street}, {city}, {state_code} {zip_code}, {country}
        const int addressComponentNumber = 4;
        string usAddressPattern = "{house_number} {street}, {city}, {state_code} {zip_code}, {country}";

        /* 
         Address after split for `,`
            addressParts[0] {house_number} {street}
            addressParts[1] {city}
            addressParts[2] {state_code} {zip_code}
            addressParts[3] {country}
        */
        var addressParts = address.Trim().Split(',');
        if (addressParts.Length != addressComponentNumber)
            throw new ArgumentNullException(nameof(address), $"The address must follow the US address pattern: {usAddressPattern}");

        var houseAndStreet = addressParts[0].Trim().Split(' ', 2);
        if (houseAndStreet.Length != 2)
            throw new ArgumentNullException(nameof(address), $"The house or street are invalid, please follow the US address pattern: {usAddressPattern}");

        if(int.TryParse(houseAndStreet[0], out int houseNoValue) == false)
            throw new ArgumentException($"The house number must be in number format.");

        var stateAndZipcode = addressParts[2].Trim().Split(' ');
        if (stateAndZipcode.Length != 2)
            throw new ArgumentNullException(nameof(address), $"The state or zip code are invalid, please follow the US address pattern: {usAddressPattern}");

        if (int.TryParse(stateAndZipcode[1], out int zipcodeValue) == false)
            throw new ArgumentException($"The zipcode must be in number format.");

        var houseNumber = houseNoValue;
        var street = houseAndStreet[1];
        var city = addressParts[1];
        var stateCode = stateAndZipcode[0];
        var zipcode = zipcodeValue;
        var country = addressParts[3];

        return new Address
        {
            InputAddress = address,
            City = city,
            Country = country,
            HouseNumber = houseNumber,
            StateCode = stateCode,
            Zipcode = zipcode,
            Street = street
        };
    }
}
