using Microsoft.EntityFrameworkCore;
using SessionHub.Api.Data;
using SessionHub.Api.Dtos;
using SessionHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<ConferenceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=sessionhub.db"));

builder.Services.AddScoped<SessionService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConferenceDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

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
    if (request.SessionId <= 0)
    {
        return Results.BadRequest("SessionId must be greater than zero.");
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
    if (sessionId <= 0)
    {
        return Results.BadRequest("SessionId must be greater than zero.");
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
