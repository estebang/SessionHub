using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SessionHub.Api.Data;
using SessionHub.Api.Dtos;
using SessionHub.Api.Models;
using SessionHub.Api.Services;

namespace SessionHub.Tests;

/// <summary>
/// Unit tests for the favorite operations in the session service.
/// </summary>
public class FavoritesServiceTests
{
    private static ConferenceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ConferenceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ConferenceDbContext(options);

        dbContext.Speakers.AddRange(
            new Speaker
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Speaker",
                Title = "Engineer",
                Company = "Contoso",
                Bio = "A test speaker.",
                PhotoUrl = ""
            });

        dbContext.Sessions.AddRange(
            new Session
            {
                Id = 1,
                Title = "Test Session",
                Summary = "Short summary",
                Description = "Detailed description",
                Track = "Architecture",
                Level = SessionLevel.Beginner,
                Room = "Room 1",
                StartTimeUtc = DateTimeOffset.UtcNow,
                EndTimeUtc = DateTimeOffset.UtcNow.AddHours(1),
                SpeakerId = 1,
                Speaker = dbContext.Speakers.Local.First()
            },
            new Session
            {
                Id = 2,
                Title = "Second Test Session",
                Summary = "Another summary",
                Description = "Another detailed description",
                Track = "Backend",
                Level = SessionLevel.Intermediate,
                Room = "Room 2",
                StartTimeUtc = DateTimeOffset.UtcNow.AddHours(2),
                EndTimeUtc = DateTimeOffset.UtcNow.AddHours(3),
                SpeakerId = 1,
                Speaker = dbContext.Speakers.Local.First()
            });

        dbContext.SaveChanges();
        return dbContext;
    }

    [Fact]
    public async Task AddFavorite_WhenSessionIsValid_ReturnsCreatedFavorite()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = new SessionService(context, new LoggerFactory().CreateLogger<SessionService>());

        // Act
        var favorite = await service.AddFavoriteAsync(1);

        // Assert
        Assert.NotNull(favorite);
        Assert.Equal(1, favorite!.SessionId);
        Assert.True(favorite.Id > 0);
    }

    [Fact]
    public async Task RemoveFavorite_WhenFavoriteExists_RemovesTheFavorite()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = new SessionService(context, new LoggerFactory().CreateLogger<SessionService>());
        await service.AddFavoriteAsync(1);

        // Act
        var removed = await service.RemoveFavoriteAsync(1);

        // Assert
        Assert.True(removed);
        Assert.Empty(await context.Favorites.ToListAsync());
    }

    [Fact]
    public async Task AddFavorite_WhenFavoriteAlreadyExists_ReturnsExistingFavoriteAndPreventsDuplicate()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = new SessionService(context, new LoggerFactory().CreateLogger<SessionService>());
        await service.AddFavoriteAsync(1);

        // Act
        var duplicateFavorite = await service.AddFavoriteAsync(1);

        // Assert
        Assert.NotNull(duplicateFavorite);
        Assert.Equal(1, duplicateFavorite!.SessionId);
        Assert.Single(await context.Favorites.ToListAsync());
    }
}
