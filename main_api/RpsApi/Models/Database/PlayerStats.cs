using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RpsApi.Models.Database;

public class PlayerStats
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Ties { get; set; }
    public int GamesPlayed { get; set; }
    /* Maybe add these later
    public int Rock { get; set; }
    public int Paper { get; set; }
    public int Scissors { get; set; } */
    // Relations
    public User User { get; set; } = null!;
    // Informational fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}