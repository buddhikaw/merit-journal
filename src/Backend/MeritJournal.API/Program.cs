using MeritJournal.API.Configuration;
using MeritJournal.API.Endpoints;
using MeritJournal.Application;
using MeritJournal.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',') ?? new[] { "http://localhost:3000" };
    options.AddPolicy("AllowSpecificOrigin",
        corsBuilder =>
        {
            corsBuilder.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromMinutes(10)); // Cache preflight requests for 10 minutes
        });
});

// Add application services
builder.Services.AddApplicationServices();

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Swagger documentation
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
// CORS must be enabled before HTTPS redirection or any authentication middleware
app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Only use HTTPS redirection in Production to avoid development issues
    // Comment out in development to avoid CORS preflight issues
    // app.UseHttpsRedirection();
}
else
{
    app.UseHttpsRedirection();
}

// Configure endpoints
app.MapGet("/", () => "Merit Journal API")
    .WithName("Home")
    .WithOpenApi();

// Map API endpoints
app.MapJournalEntryEndpoints();

app.Run();
