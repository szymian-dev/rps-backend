using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RpsApi.Models.Enums;

namespace RpsApi.Models.Database;

public class Gesture
{
    [Key]
    public int Id { get; set; }
    public GestureType? GestureType { get; set; }
    [ForeignKey("Game")]
    public int GameId { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public string FilePath { get; set; } = null!;
    // Relations
    public Game Game { get; set; } = null!;
    public User User { get; set; } = null!;
    // Informational fields
    public DateTime CreatedAt { get; set; }
}