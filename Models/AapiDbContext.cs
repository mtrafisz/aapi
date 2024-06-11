using Microsoft.EntityFrameworkCore;

namespace Aapi.Models;

public class AapiDbContext : DbContext
{
    public DbSet<Anime> Animes { get; set; }
    public DbSet<Image> Images { get; set; }

    public AapiDbContext(DbContextOptions<AapiDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anime>().HasMany(a => a.Images).WithOne(i => i.Anime).HasForeignKey(i => i.AnimeId).OnDelete(DeleteBehavior.Cascade);
    }
}
