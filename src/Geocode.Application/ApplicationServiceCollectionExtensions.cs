using Amazon.DynamoDBv2.DataModel;
using FluentValidation;
using Geocode.Application.Repository;
using Geocode.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Geocode.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IGeocodeService, GeocodeService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Transient);
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IGeocodeRepository, GeocodeRepository>();
        services.AddScoped<IDynamoDBContext, DynamoDBContext>();
        return services;
    }
}
