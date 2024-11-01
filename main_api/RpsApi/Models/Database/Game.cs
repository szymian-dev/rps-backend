using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RpsApi.Models.Enums;

namespace RpsApi.Models.Database;

public class Game
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Player1")]
    public int? Player1Id { get; set; }
    [ForeignKey("Player2")]
    public int? Player2Id { get; set; }
    public GameStatus Status { get; set; }
    [ForeignKey("Winner")]
    public int? WinnerId { get; set; }
    [ForeignKey("Loser")]
    public int? LoserId { get; set; }
    public bool? IsTie { get; set; }
    // Relations
    public User? Player1 { get; set; }
    public User? Player2 { get; set; }
    public User? Winner { get; set; }
    public User? Loser { get; set; }
    public ICollection<Gesture> Gestures { get; set; } = new List<Gesture>();
    // Informational fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}