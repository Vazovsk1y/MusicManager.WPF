﻿using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Models;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IApplicationDbContext _dbContext;

    public MovieRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Movie>> LoadAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Movies
        .ToListAsync(cancellationToken);

    public async Task<Movie?> LoadByIdAsync(MovieId id, CancellationToken cancellationToken = default)
        => await _dbContext.Movies
        .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Movie?> LoadByIdWithMoviesReleasesAsync(MovieId id, CancellationToken cancellation = default)
        => await _dbContext.Movies
        .Include(e => e.Releases)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);
}
