using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<Config> Configs { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Config>().HasData(new Config
        {
            Id = 1,
            ConfigName = "default",
            BoardSizeWidth = 5,
            BoardSizeHeight = 5,
            WinCondition = 3,
            MovePieceAfterNMoves = 2,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
        });
        
    }
    
    
}