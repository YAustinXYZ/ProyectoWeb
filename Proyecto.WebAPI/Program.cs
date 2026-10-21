using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Application.Interfaces;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Services;
using Proyecto.WebAPI.Middleware;
using Proyecto.WebAPI.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Keep framework model-binding errors in the API's documented error shape.
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(
                "Request validation failed for {Method} {Path}. Fields: {Fields}. TraceId: {TraceId}",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path,
                string.Join(", ", errors.Keys),
                context.HttpContext.TraceIdentifier);

            return new BadRequestObjectResult(new ApiErrorResponse(
                StatusCodes.Status400BadRequest,
                "REQUEST_VALIDATION_FAILED",
                "Uno o más campos de la solicitud no son válidos.",
                DateTimeOffset.UtcNow,
                context.HttpContext.TraceIdentifier,
                errors));
        };
    });

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAssetManagementService, AssetManagementService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await database.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
