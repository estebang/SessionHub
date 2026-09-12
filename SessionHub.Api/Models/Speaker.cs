namespace SessionHub.Api.Models;

public class Speaker
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    public string FullName => $"{FirstName} {LastName}";
}
