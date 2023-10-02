using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace MusicManager.Repositories.Data;

public interface IApplicationDbContext
{
    public DatabaseFacade Database { get; }

    public DbSet<Songwriter> Songwriters { get; set; }

    public DbSet<Movie> Movies { get; set; }

    public DbSet<MovieRelease> MovieReleases { get; set; }

	public DbSet<MovieReleaseLink> MovieReleaseLinks { get; set; }

	public DbSet<Compilation> Compilations { get; set; }

    public DbSet<Song> Songs { get; set; }

    public DbSet<Director> Directors { get; set; }

    public DbSet<Disc> Discs { get; set; }

    DbSet<TEntity> Set<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors 
        | DynamicallyAccessedMemberTypes.NonPublicConstructors 
        | DynamicallyAccessedMemberTypes.PublicFields 
        | DynamicallyAccessedMemberTypes.NonPublicFields 
        | DynamicallyAccessedMemberTypes.PublicProperties 
        | DynamicallyAccessedMemberTypes.NonPublicProperties 
        | DynamicallyAccessedMemberTypes.Interfaces)] TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
