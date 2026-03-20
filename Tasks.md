|---|		Task		|		Status		|		Observations		|
|---|-------------------|-------------------|---------------------------|
| 1 | Add Geocode.Contracts class library | Done | - |
| 2 | Implement Requests and Responses contracts  | Partly Done | TBD: GeocodeResponse |
| 3 | Add Geocode.Application class library | Done | - |
| 4 | Map Google Geocode API response to C# | Done | - |
| 5 | Add Address model that has the request address fields | Done | - |
| 6 | Add dependencies (DynamoDB, FluentValidation, Microsoft.Extensions.*) | Done | - |
| 7 | Implement the repository layer | Partly Done | Define db keys and adjust the logic |
| 8 | Add the repository to DI | Done | - |
| 9 | Implement the service layer | Done | - |
|10 | Add the service layer to DI | Done | - |
|11 | Add Validation to the Address model | Done | - |
|12 | Add Validator to DI | Done | - |
|13 | Add Geocode.API Web Api layer | Done | - |
|14 | Add project references to Contracts and Application layer | Done | - |
|15 | Make the project aws compatiable by adding Amazon.Lambda.AspNetCoreServer.Hosting | Done | - |
|16 | Register the AWS Lambda Hosting mechanism to DI | Done | - |
|17 | Add controller and implement the GET endpoint | Done | - |
|18 | Manually create dynamoDB and set TTL to 30 days | Done | Find a way to provision the database at deploy |
|19 | Add Unit Testing project | Done | - |
|20 | Add logging/observability | Active | - |
|21 | Add error handling | Done | - |
|22 | Add database provisioning on deploy | Active | - |
|23 | Replace class models with records for simplicity | Active | - |
|24 | Check if `[JsonPropertyName("<field_name_>")]` is necessary when first letter needs lowercase | Active | - |
|25 | When TTL expires dynamodb's record may not get deleted right away, due to the background worker that's running periodically, 
fix logic in geocode service to check for that | In progress | - |
|26 |  | Active | - |
|27 |  | Active | - |
|28 |  | Active | - |
|29 |  | Active | - |
|30 |  | Active | - |
|31 |  | Active | - |
|32 |  | Active | - |
|33 |  | Active | - |
|34 |  | Active | - |
|35 |  | Active | - |
|36 |  | Active | - |
|37 |  | Active | - |
|38 |  | Active | - |
|39 |  | Active | - |
|40 |  | Active | - |
|41 |  | Active | - |
|42 |  | Active | - |
|43 |  | Active | - |
|44 |  | Active | - |