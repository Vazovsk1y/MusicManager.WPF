using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Models;
using System.Diagnostics.CodeAnalysis;

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
