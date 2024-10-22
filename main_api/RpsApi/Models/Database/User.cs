using System.ComponentModel.DataAnnotations;

namespace RpsApi.Models.Database;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Email { get; set; } = null!;
    // Relations
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public PlayerStats? PlayerStats { get; set; }
    public ICollection<Game> GamesAsPlayer1 { get; set; } = new List<Game>();
    public ICollection<Game> GamesAsPlayer2 { get; set; } = new List<Game>();
    public ICollection<Game> GamesAsWinner { get; set; } = new List<Game>();
    public ICollection<Game> GamesAsLoser { get; set; } = new List<Game>();
    public ICollection<Gesture> Gestures { get; set; } = new List<Gesture>();
    // Informational fields
    public DateTime CreatedAt { get; set; }
}