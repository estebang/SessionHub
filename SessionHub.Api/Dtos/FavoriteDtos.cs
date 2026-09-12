namespace SessionHub.Api.Dtos;

/// <summary>
/// Represents a favorite item returned from the API.
/// </summary>
public record FavoriteDto(int Id, int SessionId, DateTimeOffset CreatedDate);

/// <summary>
/// Represents the request payload for creating a favorite.
/// </summary>
public record CreateFavoriteRequest(int SessionId);
