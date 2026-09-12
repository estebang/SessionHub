namespace SessionHub.Api.Models;

/// <summary>
/// Represents a session saved by the current demo user.
/// </summary>
public class Favorite
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// The session this favorite references.
    /// </summary>
    public Session Session { get; set; } = null!;
}
