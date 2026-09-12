using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SessionHub.Api.Data;
using SessionHub.Api.Dtos;

namespace SessionHub.Tests;

/// <summary>
/// Integration tests covering the favorite API endpoints.
/// </summary>
public class FavoritesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _databaseName = $"FavoritesIntegrationTestsDatabase-{Guid.NewGuid()}";

    public FavoritesIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<ConferenceDbContext>>();
                services.RemoveAll<ConferenceDbContext>();

                services.AddDbContext<ConferenceDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });

                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ConferenceDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        });
    }

    [Fact]
    public async Task GetFavoritesEndpoint_WhenFavoritesExist_ReturnsFavoriteSessions()
    {
        // Arrange
        var client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ConferenceDbContext>();
        db.Favorites.Add(new SessionHub.Api.Models.Favorite
        {
            SessionId = 1,
            CreatedDate = DateTimeOffset.UtcNow
        });
        db.SaveChanges();

        // Act
        var response = await client.GetAsync("/api/favorites");

        // Assert
        response.EnsureSuccessStatusCode();
        var favorites = await response.Content.ReadFromJsonAsync<List<SessionSummaryDto>>();
        Assert.NotNull(favorites);
        Assert.NotEmpty(favorites!);
        Assert.Contains(favorites, item => item.Id == 1);
    }

    [Fact]
    public async Task PostFavoriteEndpoint_WhenValidRequest_IsCreatedSuccessfully()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateFavoriteRequest(2);

        // Act
        var response = await client.PostAsJsonAsync("/api/favorites", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var favorite = await response.Content.ReadFromJsonAsync<FavoriteDto>();
        Assert.NotNull(favorite);
        Assert.Equal(2, favorite!.SessionId);
    }

    [Fact]
    public async Task DeleteFavoriteEndpoint_WhenFavoriteExists_RemovesTheFavorite()
    {
        // Arrange
        var client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ConferenceDbContext>();
        db.Favorites.Add(new SessionHub.Api.Models.Favorite
        {
            SessionId = 1,
            CreatedDate = DateTimeOffset.UtcNow
        });
        db.SaveChanges();

        // Act
        var response = await client.DeleteAsync("/api/favorites/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
