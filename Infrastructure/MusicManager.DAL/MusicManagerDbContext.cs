using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Models;
using MusicManager.Repositories.Data;
using System.Reflection;

namespace MusicManager.DAL;

public class MusicManagerDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Songwriter> Songwriters { get; set; }

    public DbSet<Movie> Movies { get; set; }

    public DbSet<MovieRelease> MovieReleases { get; set; }

    public DbSet<Compilation> Compilations { get; set; }

    public DbSet<Song> Songs { get; set; }

    public DbSet<PlaybackInfo> PlaybackInfos { get; set; }

    public DbSet<Cover> Covers { get; set; }

    public DbSet<Disc> Discs { get; set; }

    public MusicManagerDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
