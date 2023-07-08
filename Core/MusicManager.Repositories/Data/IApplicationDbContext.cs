using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Models;

namespace MusicManager.Repositories.Data;

public interface IApplicationDbContext
{
    public DbSet<Songwriter> Songwriters { get; set; }

    public DbSet<Movie> Movies { get; set; }

    public DbSet<MovieRelease> MovieReleases { get; set; }

    public DbSet<Compilation> Compilations { get; set; }

    public DbSet<Song> Songs { get; set; }

    public DbSet<PlaybackInfo> PlaybackInfos { get; set; }

    public DbSet<Cover> Covers { get; set; }
}
