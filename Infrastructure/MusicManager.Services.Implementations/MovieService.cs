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
    private readonly ISongwriterRepository _songwriterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMovieRepository _movieRepository;
    private readonly IMovieReleaseRepository _movieReleaseRepository;

    public MovieService(
        IPathToMovieService pathToMovieService,
        IMovieReleaseService movieReleaseService,
        ISongwriterRepository songwriterRepository,
        IUnitOfWork unitOfWork,
        IMovieRepository movieRepository,
        IMovieReleaseRepository movieReleaseRepository)
    {
        _pathToMovieService = pathToMovieService;
        _movieReleaseService = movieReleaseService;
        _songwriterRepository = songwriterRepository;
        _unitOfWork = unitOfWork;
        _movieRepository = movieRepository;
        _movieReleaseRepository = movieReleaseRepository;
    }

    public async Task<Result> AddExistingMovieRelease(ExistingMovieReleaseToMovieDTO dto, CancellationToken cancellationToken = default)
    {
        var movie = await _movieRepository.LoadByIdWithMoviesReleasesAsync(dto.MovieId, cancellationToken);
        if (movie is null)
        {
            return Result.Failure(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        var movieRelease = await _movieReleaseRepository.LoadWithMoviesAsync(dto.MovieReleaseLink, cancellationToken);
        if (movieRelease is null)
        {
            return Result.Failure(ServicesErrors.MovieReleaseWithPassedIdIsNotExists());
        }

        var addingResult = movie.AddRelease(movieRelease, true);
        if (addingResult.IsFailure)
        {
            return addingResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<MovieDTO>>> GetAllAsync(SongwriterId songwriterId, CancellationToken cancellation = default)
    {
        var result = new List<MovieDTO>();
        var songwriter = await _songwriterRepository.LoadByIdWithMoviesAsync(songwriterId, cancellation);

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
        var movies = await _movieRepository.LoadAllAsync(cancellationToken);
        return movies.Select(e => e.ToLookupDTO()).ToList();
    }

    public async Task<Result<MovieId>> SaveAsync(MovieAddDTO movieAddDTO, CancellationToken cancellationToken = default)
    {
        var songwriter = await _songwriterRepository.LoadByIdWithMoviesAsync(movieAddDTO.SongwriterId, cancellationToken);

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

        await _unitOfWork.SaveChangesAsync(cancellationToken);
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
        var songwriter = await _songwriterRepository.LoadByIdWithMoviesAsync(songwriterId, cancellationToken);

        if (songwriter is null)
        {
            return Result.Failure<MovieDTO>(new Error($"Songwriter with passed id is not exists in database."));
        }

        var addingResult = songwriter.AddMovie(movie, true);
        if (addingResult.IsFailure)
        {
            return Result.Failure<MovieDTO>(addingResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
