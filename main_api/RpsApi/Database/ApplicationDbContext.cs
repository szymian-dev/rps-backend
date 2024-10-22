using Microsoft.EntityFrameworkCore;
using RpsApi.Models.Database;

namespace RpsApi.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<PlayerStats> PlayerStats { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Gesture> Gestures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<Game>()
           .HasOne(g => g.Player1)
           .WithMany(u => u.GamesAsPlayer1)
           .HasForeignKey(g => g.Player1Id)
           .OnDelete(DeleteBehavior.Restrict); 

       modelBuilder.Entity<Game>()
           .HasOne(g => g.Player2)
           .WithMany(u => u.GamesAsPlayer2)
           .HasForeignKey(g => g.Player2Id)
           .OnDelete(DeleteBehavior.Restrict); 

       modelBuilder.Entity<Game>()
           .HasOne(g => g.Winner)
           .WithMany(u => u.GamesAsWinner)
           .HasForeignKey(g => g.WinnerId)
           .OnDelete(DeleteBehavior.Restrict);

       modelBuilder.Entity<Game>()
           .HasOne(g => g.Loser)
           .WithMany(u => u.GamesAsLoser)
           .HasForeignKey(g => g.LoserId)
           .OnDelete(DeleteBehavior.Restrict); 
    }
}