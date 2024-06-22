using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Aapi.Models;

public class AapiDbContext : IdentityDbContext<User>
{
    public DbSet<Anime> Animes { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Entry> Entries { get; set; }

    public AapiDbContext(DbContextOptions<AapiDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anime>().HasMany(a => a.Images).WithOne(i => i.Anime).HasForeignKey(i => i.AnimeId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>().HasOne(u => u.Avatar).WithOne(i => i.User).HasForeignKey<Image>(i => i.UserId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<User>().HasIndex(u => u.Nickname).IsUnique();

        modelBuilder.Entity<Image>().HasOne(i => i.Anime).WithMany(a => a.Images).HasForeignKey(i => i.AnimeId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Image>().HasOne(i => i.User).WithOne(u => u.Avatar).HasForeignKey<Image>(i => i.UserId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Entry>().HasKey(e => new { e.AnimeId, e.UserId });
        modelBuilder.Entity<Entry>().HasOne(e => e.Anime).WithMany(a => a.Entries).HasForeignKey(e => e.AnimeId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Entry>().HasOne(e => e.User).WithMany(u => u.Entries).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
