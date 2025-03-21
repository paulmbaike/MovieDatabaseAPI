using FluentValidation.AspNetCore;
using FluentValidation;
using MovieDatabaseAPI.API.Extensions;
using MovieDatabaseAPI.API.Middlewares;
using MovieDatabaseAPI.Infrastructure.Data;
using MovieDatabaseAPI.Infrastructure.Extensions;
using MovieDatabaseAPI.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApiDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddRateLimiting();
builder.Services.AddVersioning();
builder.Services.AddResponseCaching();

builder.Services.AddControllers();

// Register Infrastructure and Application services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Database API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseRateLimiter();
app.UseResponseCaching();
app.UseGlobalExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await Seeder.SeedDatabaseAsync(services);
}

app.Run();


public partial class Program { }