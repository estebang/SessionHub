using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SessionHub.Api.Data;
using SessionHub.Api.Dtos;
using SessionHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<ConferenceDbContext>(options =>
{
    // Migrations are authored against SQLite conventions; ignore the false-positive
    // pending-changes warning that fires when the same model runs on SQL Server in production.
    options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

    if (builder.Environment.IsEnvironment("Testing"))
    {
        options.UseInMemoryDatabase("SessionHubTestingDb");
        return;
    }

    var azureSqlConnectionString = builder.Configuration.GetConnectionString("AzureSql");
    if (!string.IsNullOrWhiteSpace(azureSqlConnectionString))
    {
        options.UseSqlServer(azureSqlConnectionString);
        return;
    }

    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=sessionhub.db");
});

builder.Services.AddScoped<SessionService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:4173", "http://localhost:4174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ConferenceDbContext>();
        db.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Use(async (context, next) =>
{
    var requestId = context.TraceIdentifier;
    var path = context.Request.Path.Value ?? string.Empty;
    var method = context.Request.Method;

    context.Response.Headers["X-Request-Id"] = requestId;
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";

    using var scope = app.Logger.BeginScope(new Dictionary<string, object>
    {
        ["RequestId"] = requestId,
        ["RequestMethod"] = method,
        ["RequestPath"] = path,
    });

    app.Logger.LogInformation("Starting request {RequestMethod} {RequestPath}", method, path);

    try
    {
        await next();
        app.Logger.LogInformation("Completed request {RequestMethod} {RequestPath} with status {StatusCode}", method, path, context.Response.StatusCode);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Unhandled exception while processing {RequestMethod} {RequestPath}", method, path);
        throw;
    }
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var requestId = context.TraceIdentifier;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        logger.LogError(exception, "Request failed for {RequestId} {RequestMethod} {RequestPath}", requestId, context.Request.Method, context.Request.Path.Value ?? string.Empty);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        context.Response.Headers["X-Request-Id"] = requestId;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = "The service could not process the request. Please try again later.",
            Instance = context.Request.Path,
        });
    });
});

app.UseCors();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.MapHealthChecks("/healthz");

app.MapGet("/api/sessions", async (SessionService service) =>
{
    var sessions = await service.GetSessionsAsync();
    return Results.Ok(sessions);
});

app.MapGet("/api/sessions/{id:int}", async (int id, SessionService service) =>
{
    var session = await service.GetSessionByIdAsync(id);
    return session is null ? Results.NotFound() : Results.Ok(session);
});

app.MapGet("/api/speakers", async (SessionService service) =>
{
    var speakers = await service.GetSpeakersAsync();
    return Results.Ok(speakers);
});

app.MapGet("/api/speakers/{id:int}", async (int id, SessionService service) =>
{
    var speaker = await service.GetSpeakerByIdAsync(id);
    return speaker is null ? Results.NotFound() : Results.Ok(speaker);
});

app.MapGet("/api/favorites", async (SessionService service) =>
{
    var favorites = await service.GetFavoriteSessionsAsync();
    return Results.Ok(favorites);
});

app.MapPost("/api/favorites", async (CreateFavoriteRequest request, SessionService service, ILogger<Program> logger) =>
{
    if (!ValidationService.IsValidSessionId(request.SessionId))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.SessionId)] = ["SessionId must be greater than zero."]
        });
    }

    var favorite = await service.AddFavoriteAsync(request.SessionId);
    if (favorite is null)
    {
        logger.LogWarning("Favorite creation rejected for session id {SessionId}", request.SessionId);
        return Results.NotFound();
    }

    return Results.Ok(favorite);
});

app.MapDelete("/api/favorites/{sessionId:int}", async (int sessionId, SessionService service, ILogger<Program> logger) =>
{
    if (!ValidationService.IsValidSessionId(sessionId))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(sessionId)] = ["SessionId must be greater than zero."]
        });
    }

    var removed = await service.RemoveFavoriteAsync(sessionId);
    if (!removed)
    {
        logger.LogInformation("Delete favorite request for missing session id {SessionId}", sessionId);
        return Results.NotFound();
    }

    return Results.NoContent();
});

app.Run();

public static class ValidationService
{
    public static bool IsValidSessionId(int sessionId) => sessionId > 0;
}

public partial class Program { }
