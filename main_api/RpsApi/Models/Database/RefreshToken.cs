using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RpsApi.Models.Database;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    // Relations
    public User User { get; set; } = null!;
    // Informational fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}