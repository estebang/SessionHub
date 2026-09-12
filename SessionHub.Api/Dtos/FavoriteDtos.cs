using System.ComponentModel.DataAnnotations;

namespace SessionHub.Api.Dtos;

/// <summary>
/// Represents a favorite item returned from the API.
/// </summary>
public record FavoriteDto(int Id, int SessionId, DateTimeOffset CreatedDate);

/// <summary>
/// Represents the request payload for creating a favorite.
/// </summary>
public record CreateFavoriteRequest(
    [property: Range(1, int.MaxValue, ErrorMessage = "SessionId must be greater than zero.")] int SessionId);
