using Geocode.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddDatabase();
builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
