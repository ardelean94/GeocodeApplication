
using System.Net;

namespace Geocode.Application.UnitTests.HttpHandler;

internal sealed class HttpGeocodeMockHandler : HttpMessageHandler
{
    private readonly HttpStatusCode statusCode;

    public HttpGeocodeMockHandler(HttpStatusCode statusCode)
    {
        this.statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = statusCode
        });
    }
}
