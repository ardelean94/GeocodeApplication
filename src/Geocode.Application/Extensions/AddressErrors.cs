using Geocode.Application.Abstractions;

namespace Geocode.Application.Extensions;

public static class AddressErrors
{
    const string usAddressPattern = "{house_number} {street}, {city}, {state_code} {zip_code}, {country}";

    public static readonly Error NullOrEmpty = new(
        "Address.NullOrEmpty",
        "Address cannot be null or empty.");

    public static readonly Error InvalidAddress = new(
        "Address.InvalidAddress",
        $"The address must follow the US address pattern: {usAddressPattern}.");

    public static readonly Error InvalidHouseOrStreet = new(
        "Address.InvalidHouseOrStreet",
        $"The house or street are invalid, please follow the US address pattern: {usAddressPattern}.");

    public static readonly Error InvalidHouseNumber = new(
        "Address.InvalidHouseNumber",
        "The house number must be in number format.");

    public static readonly Error InvalidStateOrZipcode = new(
        "Address.InvalidStateOrZipcode",
        $"The state or zip code are invalid, please follow the US address pattern: {usAddressPattern}.");

    public static readonly Error InvalidZipcode = new(
        "Address.InvalidZipcode",
        "The zipcode must be in number format.");
}
