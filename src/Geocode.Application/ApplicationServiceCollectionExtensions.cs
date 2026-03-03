using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FluentValidation;
using Geocode.Application.Repository;
using Geocode.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Geocode.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        var awsOptions = config.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);
        services.AddSingleton<IGeocodeService, GeocodeService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Transient);
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
