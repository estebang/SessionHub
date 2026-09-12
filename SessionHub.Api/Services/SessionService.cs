using Microsoft.EntityFrameworkCore;
using SessionHub.Api.Data;
using SessionHub.Api.Dtos;
using SessionHub.Api.Models;

namespace SessionHub.Api.Services;

public class SessionService
{
    private readonly ConferenceDbContext _dbContext;

    public SessionService(ConferenceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SessionSummaryDto>> GetSessionsAsync()
    {
        return await _dbContext.Sessions
            .AsNoTracking()
            .Include(session => session.Speaker)
            .OrderBy(session => session.StartTimeUtc)
            .Select(session => SessionMapping.ToSummaryDto(session))
            .ToListAsync();
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
}
