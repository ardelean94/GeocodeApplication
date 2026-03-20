using Geocode.Application.Abstractions;

namespace Geocode.Application.ResultErrors;

public static class HttpErrors
{
    public static readonly Error HttpRequestFailed = new(
        "HttpErrors.HttpRequestFailed",
        "Http request failed.");

    public static readonly Error TaskCancelled = new(
        "HttpErrors.TaskCancelled",
        "Task cancelled.");
}
