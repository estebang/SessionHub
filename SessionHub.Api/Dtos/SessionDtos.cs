using SessionHub.Api.Models;

namespace SessionHub.Api.Dtos;

public record SessionSummaryDto(
    int Id,
    string Title,
    string Summary,
    string Track,
    string Level,
    string Room,
    DateTimeOffset StartTimeUtc,
    DateTimeOffset EndTimeUtc,
    SpeakerSummaryDto Speaker);

public record SessionDetailDto(
    int Id,
    string Title,
    string Summary,
    string Description,
    string Track,
    string Level,
    string Room,
    DateTimeOffset StartTimeUtc,
    DateTimeOffset EndTimeUtc,
    SpeakerSummaryDto Speaker);

public record SpeakerSummaryDto(int Id, string Name, string Title, string Company);

public record SpeakerDto(int Id, string Name, string Title, string Company, string Bio, string PhotoUrl);

public static class SessionMapping
{
    public static SessionSummaryDto ToSummaryDto(Session session) =>
        new(
            session.Id,
            session.Title,
            session.Summary,
            session.Track,
            session.Level.ToString(),
            session.Room,
            session.StartTimeUtc,
            session.EndTimeUtc,
            new SpeakerSummaryDto(
                session.Speaker.Id,
                session.Speaker.FullName,
                session.Speaker.Title,
                session.Speaker.Company));

    public static SessionDetailDto ToDetailDto(Session session) =>
        new(
            session.Id,
            session.Title,
            session.Summary,
            session.Description,
            session.Track,
            session.Level.ToString(),
            session.Room,
            session.StartTimeUtc,
            session.EndTimeUtc,
            new SpeakerSummaryDto(
                session.Speaker.Id,
                session.Speaker.FullName,
                session.Speaker.Title,
                session.Speaker.Company));

    public static SpeakerDto ToSpeakerDto(Speaker speaker) =>
        new(
            speaker.Id,
            speaker.FullName,
            speaker.Title,
            speaker.Company,
            speaker.Bio,
            speaker.PhotoUrl);
}
