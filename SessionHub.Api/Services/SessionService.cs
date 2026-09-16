using Microsoft.EntityFrameworkCore;
using SessionHub.Api.Data;
using SessionHub.Api.Dtos;
using SessionHub.Api.Models;

namespace SessionHub.Api.Services;

public class SessionService
{
    private readonly ConferenceDbContext _dbContext;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ConferenceDbContext dbContext, ILogger<SessionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<SessionSummaryDto>> GetSessionsAsync()
    {
        var sessions = await _dbContext.Sessions
            .AsNoTracking()
            .Include(session => session.Speaker)
            .ToListAsync();

        return sessions
            .OrderBy(session => session.StartTimeUtc)
            .Select(session => SessionMapping.ToSummaryDto(session))
            .ToList();
    }

    public async Task<SessionDetailDto?> GetSessionByIdAsync(int id)
    {
        var session = await _dbContext.Sessions
            .AsNoTracking()
            .Include(session => session.Speaker)
            .FirstOrDefaultAsync(item => item.Id == id);

        return session is null ? null : SessionMapping.ToDetailDto(session);
    }

    public async Task<List<SpeakerDto>> GetSpeakersAsync()
    {
        return await _dbContext.Speakers
            .AsNoTracking()
            .OrderBy(speaker => speaker.LastName)
            .Select(speaker => SessionMapping.ToSpeakerDto(speaker))
            .ToListAsync();
    }

    public async Task<SpeakerDto?> GetSpeakerByIdAsync(int id)
    {
        var speaker = await _dbContext.Speakers
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        return speaker is null ? null : SessionMapping.ToSpeakerDto(speaker);
    }

    /// <summary>
    /// Gets all favorited sessions for the current single-user demo model.
    /// </summary>
    public async Task<List<SessionSummaryDto>> GetFavoriteSessionsAsync()
    {
        var favoriteSessionIds = await _dbContext.Favorites
            .AsNoTracking()
            .Select(favorite => favorite.SessionId)
            .ToListAsync();

        if (favoriteSessionIds.Count == 0)
        {
            return new List<SessionSummaryDto>();
        }

        return await _dbContext.Sessions
            .AsNoTracking()
            .Include(session => session.Speaker)
            .Where(session => favoriteSessionIds.Contains(session.Id))
            .OrderBy(session => session.StartTimeUtc)
            .Select(session => SessionMapping.ToSummaryDto(session))
            .ToListAsync();
    }

    /// <summary>
    /// Creates a favorite for the provided session id unless it already exists.
    /// </summary>
    public async Task<FavoriteDto?> AddFavoriteAsync(int sessionId)
    {
        if (sessionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sessionId));
        }

        var sessionExists = await _dbContext.Sessions.AnyAsync(session => session.Id == sessionId);
        if (!sessionExists)
        {
            _logger.LogWarning("Favorite requested for unknown session id {SessionId}", sessionId);
            return null;
        }

        var existingFavorite = await _dbContext.Favorites
            .AsNoTracking()
            .FirstOrDefaultAsync(favorite => favorite.SessionId == sessionId);

        if (existingFavorite is not null)
        {
            _logger.LogInformation("Duplicate favorite attempt for session id {SessionId}", sessionId);
            return new FavoriteDto(existingFavorite.Id, existingFavorite.SessionId, existingFavorite.CreatedDate);
        }

        var favorite = new Favorite
        {
            SessionId = sessionId,
            CreatedDate = DateTimeOffset.UtcNow,
        };

        _dbContext.Favorites.Add(favorite);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Favorite added for session id {SessionId}", sessionId);
        return new FavoriteDto(favorite.Id, favorite.SessionId, favorite.CreatedDate);
    }

    /// <summary>
    /// Removes a favorite by session id.
    /// </summary>
    public async Task<bool> RemoveFavoriteAsync(int sessionId)
    {
        if (sessionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sessionId));
        }

        var favorite = await _dbContext.Favorites
            .FirstOrDefaultAsync(item => item.SessionId == sessionId);

        if (favorite is null)
        {
            _logger.LogInformation("Remove favorite called for missing session id {SessionId}", sessionId);
            return false;
        }

        _dbContext.Favorites.Remove(favorite);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Favorite removed for session id {SessionId}", sessionId);
        return true;
    }
}
