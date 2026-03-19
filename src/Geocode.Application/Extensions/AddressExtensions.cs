using Geocode.Application.Abstractions;
using Geocode.Application.Models;

namespace Geocode.Application.Extensions;

internal static class AddressExtensions
{
    internal static Result<Address> FormatAddress(this string address)
    {
        if (string.IsNullOrEmpty(address))
            return Result.Failure<Address>(AddressErrors.NullOrEmpty);

        //  US address pattern:
        //  {house_number} {street}, {city}, {state_code} {zip_code}, {country}
        const int addressComponentNumber = 4;

        /* 
         Address after split for `,`
            addressParts[0] {house_number} {street}
            addressParts[1] {city}
            addressParts[2] {state_code} {zip_code}
            addressParts[3] {country}
        */
        var addressParts = address.Trim().Split(',');
        if (addressParts.Length != addressComponentNumber)
            return Result.Failure<Address>(AddressErrors.InvalidAddress);

        var houseAndStreet = addressParts[0].Trim().Split(' ', 2);
        if (houseAndStreet.Length != 2)
            return Result.Failure<Address>(AddressErrors.InvalidHouseOrStreet);

        if (int.TryParse(houseAndStreet[0], out int houseNoValue) == false)
            return Result.Failure<Address>(AddressErrors.InvalidHouseNumber);

        var stateAndZipcode = addressParts[2].Trim().Split(' ');
        if (stateAndZipcode.Length != 2)
            return Result.Failure<Address>(AddressErrors.InvalidStateOrZipcode);

        if (int.TryParse(stateAndZipcode[1], out int zipcodeValue) == false)
            return Result.Failure<Address>(AddressErrors.InvalidZipcode);

        var houseNumber = houseNoValue;
        var street = houseAndStreet[1];
        var city = addressParts[1];
        var stateCode = stateAndZipcode[0];
        var zipcode = zipcodeValue;
        var country = addressParts[3];

        var output = new Address
        {
            InputAddress = address,
            City = city,
            Country = country,
            HouseNumber = houseNumber,
            StateCode = stateCode,
            Zipcode = zipcode,
            Street = street
        };

        return Result.Success(output);
    }
}
