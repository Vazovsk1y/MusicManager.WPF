using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Models;

namespace MusicManager.Repositories.Data;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }

    DbSet<Songwriter> Songwriters { get; set; }

    DbSet<Movie> Movies { get; set; }

    DbSet<MovieRelease> MoviesReleases { get; set; }

	DbSet<MovieReleaseLink> MovieReleaseLinks { get; set; }

	DbSet<Compilation> Compilations { get; set; }

    DbSet<Song> Songs { get; set; }

    DbSet<Director> Directors { get; set; }

    DbSet<Disc> Discs { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
