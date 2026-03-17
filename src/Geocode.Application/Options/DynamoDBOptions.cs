namespace Geocode.Application.Options;

public class DynamoDBOptions
{
    public int CacheTTLDays { get; set; } = 0;
    public string? CreatedAtDateFormat { get; set; }
}
