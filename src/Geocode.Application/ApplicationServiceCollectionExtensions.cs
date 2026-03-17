using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FluentValidation;
using Geocode.Application.Options;
using Geocode.Application.Repository;
using Geocode.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Geocode.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        var awsOptions = config.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);
        services.AddSingleton<IGeocodeService, GeocodeService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Transient);

        services.AddOptions<DynamoDBOptions>()
            .BindConfiguration("DynamoDBInfo")
            .Validate(options =>
                !string.IsNullOrEmpty(options.CreatedAtDateFormat),
                "DynamoDB Options failed validation.");

        services.AddOptions<GeocodeOptions>()
            .BindConfiguration("GeocodeInfo")
            .Validate(options => 
                !string.IsNullOrEmpty(options.ApiKey) &&
                !string.IsNullOrEmpty(options.GeocodeUrl) &&
                !string.IsNullOrEmpty(options.GeocodeUrlResponseFormat),
                "Geocode Options failed validation.");

        services.AddHttpClient<GeocodeHttp>((serviceProvider, client) =>
        {
            var geocodeInfo = serviceProvider.GetRequiredService<IOptions<GeocodeOptions>>();

            var geocodeUrl = Path.Combine(
                geocodeInfo.Value.GeocodeUrl,
                geocodeInfo.Value.GeocodeUrlResponseFormat);

            client.BaseAddress = new Uri(geocodeUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<IGeocodeRepository, GeocodeRepository>();
        services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
        return services;
    }
}
