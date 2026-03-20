using Geocode.Application.Abstractions;
using Geocode.Application.Models;
using Geocode.Application.ResultErrors;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Geocode.Application.Services;

public class GeocodeHttp(HttpClient httpClient, ILogger<GeocodeHttp> logger) : IGeocodeHttp
{
    public async Task<Result<GoogleGeocodeModel?>> FetchGeocodeDataAsync(string address, string apiKey, CancellationToken token = default)
    {
        var encodedAddress = Uri.EscapeDataString(address);
        var requestUrl = $"?address={encodedAddress}&key={apiKey}";
        HttpResponseMessage responseMessage;
        GoogleGeocodeModel? output = null;

        try
        {
            responseMessage = await httpClient.GetAsync(requestUrl, token);
            responseMessage.EnsureSuccessStatusCode();

            using var contentStream = await responseMessage.Content.ReadAsStreamAsync(token);
            output = await DeserializeResponseAsync(contentStream, token);
        }
        catch (HttpRequestException exReq)
        {
            logger.LogError("Http Request Exception: {Message}", exReq.Message);
            return Result.Failure<GoogleGeocodeModel?>(HttpErrors.HttpRequestFailed);
        }
        catch (TaskCanceledException exTsk)
        {
            logger.LogError("Task Canceled Exception: {Message}", exTsk.Message);
            return Result.Failure<GoogleGeocodeModel?>(HttpErrors.TaskCancelled);
        }

        return Result.Success(output);
    }

    private async Task<GoogleGeocodeModel?> DeserializeResponseAsync(Stream stream, CancellationToken token)
    {
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        GoogleGeocodeModel? geocode = null;

        try
        {
            geocode = await JsonSerializer.DeserializeAsync<GoogleGeocodeModel>(
                stream,
                jsonOptions,
                token);
        }
        catch (JsonException ex)
        {
            logger.LogError("Failed to deserialize google geocode response: {Message}", ex.Message);
            throw;
        }
        return geocode;
    }
}
