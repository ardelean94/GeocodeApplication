using Geocode.Application.Abstractions;

namespace Geocode.Application.ResultErrors;

public class GeocodeRepositoryErrors
{
    public static readonly Error FailLoadData = new(
        "GeocodeRepository.NullOrEmpty",
        "Fail to load data.");

    public static readonly Error FailCreateRecord = new(
        "GeocodeRepository.FailCreateRecord",
        "Fail to create record.");
}
