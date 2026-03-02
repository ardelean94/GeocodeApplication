# GeocodeApplication

Serverless Geocoding Cache

A cloud-native solution that wraps the Google Geocoding API with a serverless caching layer.
The application routes geocoding requests through an AWS Lambda function.
Instead of calling Geocoding API on every request, the system:

- Checks DynamoDB for a cached result
- If found (and not expired), returns the cached response
- If not found, calls Google Geocoding API
- Stores the result in DynamoDB with a 30-day TTL
- Returns the response to the caller

Why it matters?

The Geocoding API charges per request and also there's a rate limited inplace, repeated requests on the 
same data results in unnecessary API calls which leads to costs increase.
By caching the response it results in reducing API calls and cost, improving the response time and 
minimizing the 3rd-party dependency usage

Technologies:
- .NET 8 / C#
- AWS Lambda
- AWS DynamoDB
- Google Geocoding API