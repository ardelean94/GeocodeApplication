using FluentAssertions;
using Geocode.Application.Abstractions;
using Geocode.Application.Models;
using Geocode.Application.Options;
using Geocode.Application.Repository;
using Geocode.Application.ResultErrors;
using Geocode.Application.Services;
using Geocode.Application.UnitTests.Helpers;
using Geocode.Application.UnitTests.HttpHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ExceptionExtensions;
using System.Net;

namespace Geocode.Application.UnitTests.Services;

public class GeocodeServiceTests
{
    private readonly IGeocodeRepository geocodeRepositoryMock;
    private readonly IGeocodeHttp geocodeHttpMock;
    private readonly ILogger<GeocodeService> loggerMock;
    private readonly IOptions<GeocodeOptions> geocodeOptionsMock;
    private readonly IOptions<DynamoDBOptions> dynamoOptionsMock;
    private readonly GeocodeService _sut;

    public GeocodeServiceTests()
    {
        geocodeRepositoryMock = Substitute.For<IGeocodeRepository>();
        geocodeHttpMock = Substitute.For<IGeocodeHttp>();
        loggerMock = NullLogger<GeocodeService>.Instance;

        geocodeOptionsMock = Substitute.For<IOptions<GeocodeOptions>>();
        geocodeOptionsMock.Value.Returns(new GeocodeOptions());

        dynamoOptionsMock = Substitute.For<IOptions<DynamoDBOptions>>();
        dynamoOptionsMock.Value.Returns(new DynamoDBOptions());

        _sut = new GeocodeService(
            loggerMock,
            geocodeRepositoryMock,
            geocodeHttpMock,
            dynamoOptionsMock,
            geocodeOptionsMock);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Get_ShouldReturnResultFailuire_WhenInputNullOrEmpty(string? input)
    {
        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == AddressErrors.NullOrEmpty
        );
    }

    [Theory]
    [InlineData("41 Biloxi")]
    [InlineData("160 North Rd, Port Angeles, WA")]
    [InlineData("160 North Rd, Port Angeles, WA, United States, USA")]
    public async Task Get_ShouldReturnResultFailuire_WhenInvalidAddress(string input)
    {
        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == AddressErrors.InvalidAddress
        );
    }

    [Theory]
    [InlineData(" North Rd, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnResultFailuire_WhenMissingHouseNumber(string input)
    {
        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == AddressErrors.InvalidHouseNumber
        );
    }

    [Theory]
    [InlineData(" 160, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnResultFailuire_WhenMissingStreet(string input)
    {
        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == AddressErrors.InvalidHouseOrStreet
        );
    }

    [Theory]
    [InlineData("160Bulli, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnResultFailuire_WhenNoSpaceBetweenStreetAndHouse(string input)
    {
        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == AddressErrors.InvalidHouseOrStreet
        );
    }

    [Theory]
    [InlineData("160 Bulli, Port Angeles, WA, United States")]
    public async Task Get_ShouldReturnResultFailuire_WhenMissingZipcode(string input)
    {
        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == AddressErrors.InvalidStateOrZipcode
        );
    }

    [Theory]
    [InlineData("160 Bulli, Port Angeles, 34567, United States")]
    public async Task Get_ShouldReturnResultFailuire_WhenMissingStateCode(string input)
    {
        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == AddressErrors.InvalidStateOrZipcode
        );
    }

    [Theory]
    [InlineData("160 Bulli, Port Angeles, 34567 34567 , United States")]
    public async Task Get_ShouldReturnResultFailuire_WhenInvalidStateCode(string input)
    {
        //  Arrange

        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == AddressErrors.InvalidStateCode
        );
    }

    [Theory]
    [InlineData("160 North Rd, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnResultSuccess_WhenCacheHit(string input)
    {
        //  Arrange
        var cacheSample = GeocodeHelpers.GetGeocodeCacheSample();

        geocodeRepositoryMock
            .Get(Arg.Any<string>())
            .Returns(Result.Success<GeocodeCache?>(cacheSample));

        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsSuccess &&
            pred.Value != null
        );
    }

    [Theory]
    [InlineData("160 North Rd, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnResultFailure_WhenCacheCheck(string input)
    {
        //  Arrange
        geocodeRepositoryMock
            .Get(Arg.Any<string>())
            .Returns(Result.Failure<GeocodeCache?>(GeocodeRepositoryErrors.FailLoadData));

        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == GeocodeRepositoryErrors.FailLoadData
        );
    }

    [Theory]
    [InlineData("160 North Rd, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnSuccess_WhenCacheHit(string input)
    {
        //  Arrange
        GeocodeCache geocodeCache = GeocodeHelpers.GetGeocodeCacheSample();

        geocodeRepositoryMock
            .Get(Arg.Any<string>())
            .Returns(Result.Success<GeocodeCache?>(geocodeCache));

        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsSuccess &&
            pred.Value == geocodeCache
        );
    }

    [Theory]
    [InlineData("160 North Rd, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnFailure_WhenCacheMiss_HttpRequestFailed(string input)
    {
        //  Arrange
        GeocodeCache geocodeCache = null;

        geocodeRepositoryMock
            .Get(Arg.Any<string>())
            .Returns(Result.Success<GeocodeCache?>(geocodeCache));

        geocodeHttpMock
            .FetchGeocodeDataAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result.Failure<GoogleGeocodeModel?>(HttpErrors.HttpRequestFailed));

        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == HttpErrors.HttpRequestFailed
        );
    }

    [Theory]
    [InlineData("160 North Rd, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnFailure_WhenCacheMiss_TaskCancelled(string input)
    {
        //  Arrange
        GeocodeCache geocodeCache = null;

        geocodeRepositoryMock
            .Get(Arg.Any<string>())
            .Returns(Result.Success<GeocodeCache?>(geocodeCache));

        geocodeHttpMock
            .FetchGeocodeDataAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result.Failure<GoogleGeocodeModel?>(HttpErrors.TaskCancelled));

        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == HttpErrors.TaskCancelled
        );
    }

    [Theory]
    [InlineData("160 North Rd, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnFailure_WhenCacheMiss_FailedCaching(string input)
    {
        //  Arrange
        GeocodeCache geocodeCache = null;
        GoogleGeocodeModel geocodeModel = new();

        geocodeRepositoryMock
            .Get(Arg.Any<string>())
            .Returns(Result.Success<GeocodeCache?>(geocodeCache));

        geocodeRepositoryMock
            .Create(Arg.Any<GeocodeCache>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<bool>(GeocodeRepositoryErrors.FailCreateRecord));

        geocodeHttpMock
            .FetchGeocodeDataAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result.Success<GoogleGeocodeModel?>(geocodeModel));

        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsFailure &&
            pred.Error == GeocodeRepositoryErrors.FailCreateRecord
        );
    }

    [Theory]
    [InlineData("160 North Rd, Port Angeles, WA 34567, United States")]
    public async Task Get_ShouldReturnSuccess_WhenCacheMiss_SuccessCaching(string input)
    {
        //  Arrange
        GeocodeCache geocodeCache = null;
        GeocodeCache geocodeNewRecord = null;
        GoogleGeocodeModel geocodeModel = new();

        geocodeRepositoryMock
            .Get(Arg.Any<string>())
            .Returns(Result.Success<GeocodeCache?>(geocodeCache));

        geocodeRepositoryMock
            .Create(Arg.Do<GeocodeCache>(r => geocodeNewRecord = r), Arg.Any<CancellationToken>())
            .Returns(Result.Success<bool>(true));

        geocodeHttpMock
            .FetchGeocodeDataAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result.Success<GoogleGeocodeModel?>(geocodeModel));

        //  Act
        var output = await _sut.Get(input);

        //  Assert
        output.Should().Match<Result<GeocodeCache>>(pred =>
            pred.IsSuccess &&
            pred.Value == geocodeNewRecord
        );
    }
}
