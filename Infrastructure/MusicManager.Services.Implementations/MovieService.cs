using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IPathToMovieService _pathToMovieService;
    private readonly IMovieReleaseService _movieReleaseService;
    private readonly IApplicationDbContext _dbContext;

    public MovieService(
        IPathToMovieService pathToMovieService,
        IMovieReleaseService movieReleaseService,
        IApplicationDbContext dbContext)
    {
        _pathToMovieService = pathToMovieService;
        _movieReleaseService = movieReleaseService;
        _dbContext = dbContext;
    }

    public async Task<Result> AddExistingMovieRelease(ExistingMovieReleaseToMovieDTO dto, CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext
            .Movies
            .Include(e => e.Releases)
            .ThenInclude(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == dto.MovieId, cancellationToken);

        if (movie is null)
        {
            return Result.Failure(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        var movieRelease = await _dbContext
            .MovieReleases
            .Include(e => e.Movies)
            .ThenInclude(e => e.Releases)
            .SingleOrDefaultAsync(e => e.Id == dto.DiscId, cancellationToken);

        if (movieRelease is null)
        {
            return Result.Failure(ServicesErrors.MovieReleaseWithPassedIdIsNotExists());
        }

        var addingResult = movie.AddRelease(movieRelease, true);
        if (addingResult.IsFailure)
        {
            return addingResult;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<MovieDTO>>> GetAllAsync(SongwriterId songwriterId, CancellationToken cancellation = default)
    {
        var result = new List<MovieDTO>();
        var songwriter = await _dbContext
            .Songwriters
            .AsNoTracking()
            .Include(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == songwriterId, cancellation);

        if (songwriter is null)
        {
            return Result.Failure<IEnumerable<MovieDTO>>(new Error($"Songwriter with passed id is not exists in database."));
        }

        var movies = songwriter.Movies;
        foreach (var movie in movies)
        {
            var moviesReleasesResult = await _movieReleaseService.GetAllAsync(movie.Id, cancellation);
            if (moviesReleasesResult.IsFailure)
            {
                return Result.Failure<IEnumerable<MovieDTO>>(moviesReleasesResult.Error);
            }

            result.Add(movie.ToDTO() with
            {
                MovieReleasesDTOs = moviesReleasesResult.Value
            });
        }

        return result;
    }

    public async Task<Result<IEnumerable<MovieLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Movies
            .AsNoTracking()
            .Select(e => e.ToLookupDTO())
            .ToListAsync(cancellationToken); 
    }

    public async Task<Result<MovieId>> SaveAsync(MovieAddDTO movieAddDTO, CancellationToken cancellationToken = default)
    {
        var songwriter = await _dbContext
            .Songwriters
            .Include(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movieAddDTO.SongwriterId, cancellationToken);

        if (songwriter is null)
        {
            return Result.Failure<MovieId>(ServicesErrors.SongwriterWithPassedIdIsNotExists());
        }

        var movieCreationResult = Movie.Create(
            movieAddDTO.SongwriterId,
            movieAddDTO.Title,
            movieAddDTO.ProductionYear,
            movieAddDTO.ProductionCountry
            );

        if (movieCreationResult.IsFailure)
        {
            return Result.Failure<MovieId>(movieCreationResult.Error);
        }

        var movie = movieCreationResult.Value;
        var addingResult = songwriter.AddMovie(movie, true);

        if (addingResult.IsFailure)
        {
            return Result.Failure<MovieId>(addingResult.Error);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(movie.Id);
    }

    public async Task<Result<MovieDTO>> SaveFromFolderAsync(MovieFolder movieFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default)
    {
        var movieResult = await _pathToMovieService
            .GetEntityAsync(movieFolder.Path, songwriterId)
            .ConfigureAwait(false);

        if (movieResult.IsFailure)
        {
            return Result.Failure<MovieDTO>(movieResult.Error);
        }

        var movie = movieResult.Value;
        var songwriter = await _dbContext
            .Songwriters
            .Include(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movie.SongwriterId, cancellationToken);

        if (songwriter is null)
        {
            return Result.Failure<MovieDTO>(ServicesErrors.SongwriterWithPassedIdIsNotExists());
        }

        var addingResult = songwriter.AddMovie(movie, true);
        if (addingResult.IsFailure)
        {
            return Result.Failure<MovieDTO>(addingResult.Error);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var moviesReleasesDtos = new List<MovieReleaseDTO>();
        foreach (var movieReleaseFolder in movieFolder.MoviesReleasesFolders)
        {
            var result = await _movieReleaseService.SaveFromFolderAsync(movieReleaseFolder, movie.Id, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<MovieDTO>(result.Error);
            }

            moviesReleasesDtos.Add(result.Value);
        }

        return movie.ToDTO() with
        {
            MovieReleasesDTOs = moviesReleasesDtos,
        };
    }
}
