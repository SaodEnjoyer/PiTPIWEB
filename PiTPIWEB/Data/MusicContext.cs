using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Reflection.Emit;

public class MusicContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Song> Songs { get; set; }

    public MusicContext(DbContextOptions<MusicContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasKey(a => a.Id);
        modelBuilder.Entity<Song>().HasKey(s => s.Id);

        modelBuilder.Entity<Song>()
            .HasOne<Author>()
            .WithMany()
            .HasForeignKey(s => s.AuthorId);
    }
}
