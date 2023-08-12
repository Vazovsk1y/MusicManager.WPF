﻿using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class MovieReleaseService : IMovieReleaseService
{
    private readonly IPathToMovieReleaseService _pathToMovieReleaseService;
    private readonly ISongService _songService;
    private readonly IApplicationDbContext _dbContext;

    public MovieReleaseService(
        IPathToMovieReleaseService pathToMovieReleaseService,
        ISongService songService,
        IApplicationDbContext dbContext)
    {
        _pathToMovieReleaseService = pathToMovieReleaseService;
        _songService = songService;
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<MovieReleaseDTO>>> GetAllAsync(MovieId movieId, CancellationToken cancellationToken = default)
    {
        var result = new List<MovieReleaseDTO>();
        var movie = await _dbContext
            .Movies
            .Include(e => e.Releases)
            .ThenInclude(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movieId, cancellationToken);

        if (movie is null)
        {
            return Result.Failure<IEnumerable<MovieReleaseDTO>>(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        var moviesReleases = movie.Releases;
        foreach (var movieRelease in moviesReleases)
        {
            var songsResult = await _songService.GetAllAsync(movieRelease.Id, cancellationToken);
            if (songsResult.IsFailure)
            {
                return Result.Failure<IEnumerable<MovieReleaseDTO>>(songsResult.Error);
            }

            result.Add(movieRelease.ToDTO() with
            {
                SongDTOs = songsResult.Value
            });
        }

        return result;
    }

    public async Task<Result<DiscId>> SaveAsync(MovieReleaseAddDTO movieReleaseAddDTO, CancellationToken cancellationToken = default)
    {
        bool hasDuplicates = movieReleaseAddDTO.MoviesLinks.GroupBy(x => x).Any(g => g.Count() > 1);
        if (hasDuplicates)
        {
            return Result.Failure<DiscId>(new("Can't add the same movie release to the movie twice.\nMovies links contains duplicates"));
        }

        var movies = await _dbContext
            .Movies
            .Include(e => e.Releases)
            .ThenInclude(e => e.Movies)
            .Where(e => movieReleaseAddDTO.MoviesLinks.Contains(e.Id))
            .ToListAsync(cancellationToken);

        var creationResult = MovieRelease.Create(
            movieReleaseAddDTO.DiscType,
            movieReleaseAddDTO.Identifier
            );

        if (creationResult.IsFailure)
        {
            return Result.Failure<DiscId>(creationResult.Error);
        }

        var movieRelease = creationResult.Value;
        foreach (var movie in movies)
        {
            var addingResult = movie.AddRelease(movieRelease, true);
            if (addingResult.IsFailure)
            {
                return Result.Failure<DiscId>(addingResult.Error);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(movieRelease.Id);
    }

    public async Task<Result<MovieReleaseDTO>> SaveFromFolderAsync(DiscFolder movieReleaseFolder, MovieId movieId, CancellationToken cancellationToken = default)
    {
        var movieReleaseResult = await _pathToMovieReleaseService
            .GetEntityAsync(movieReleaseFolder.Path)
            .ConfigureAwait(false);

        if (movieReleaseResult.IsFailure)
        {
            return Result.Failure<MovieReleaseDTO>(movieReleaseResult.Error);
        }

        var movieRelease = movieReleaseResult.Value;
        var movie = await _dbContext
            .Movies
            .Include(e => e.Releases)
            .ThenInclude(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movieId, cancellationToken);

        if (movie is null)
        {
            return Result.Failure<MovieReleaseDTO>(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        foreach (var coverPath in movieReleaseFolder.CoversPaths)
        {
            movieRelease.AddCover(coverPath);
        }

        var addingResult = movie.AddRelease(movieRelease);
        if (addingResult.IsFailure)
        {
            return Result.Failure<MovieReleaseDTO>(addingResult.Error);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var songsDtos = new List<SongDTO>();    
        foreach (var songFile in movieReleaseFolder.Songs)
        {
            var result = await _songService.SaveFromFileAsync(songFile, movieRelease.Id, true, cancellationToken);

            if (result.IsFailure)
            {
                return Result.Failure<MovieReleaseDTO>(result.Error);
            }

            songsDtos.AddRange(result.Value);
        }

        return movieRelease.ToDTO() with
        {
            SongDTOs = songsDtos,
        };
    }
}
