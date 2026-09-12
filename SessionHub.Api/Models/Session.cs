namespace SessionHub.Api.Models;

public enum SessionLevel
{
    Beginner,
    Intermediate,
    Advanced
}

public class Session
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Track { get; set; } = string.Empty;
    public SessionLevel Level { get; set; }
    public string Room { get; set; } = string.Empty;
    public DateTimeOffset StartTimeUtc { get; set; }
    public DateTimeOffset EndTimeUtc { get; set; }
    public int SpeakerId { get; set; }
    public Speaker Speaker { get; set; } = null!;
}
