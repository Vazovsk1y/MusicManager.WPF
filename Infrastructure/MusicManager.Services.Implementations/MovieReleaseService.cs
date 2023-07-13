using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Implementations;

public class MovieReleaseService : IMovieReleaseService
{
    private readonly IPathToMovieReleaseService _pathToMovieReleaseService;
    private readonly ISongService _songService;
    private readonly IMovieRepository _movieRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MovieReleaseService(
        IPathToMovieReleaseService pathToMovieReleaseService,
        ISongService songService,
        IMovieRepository movieRepository,
        IUnitOfWork unitOfWork)
    {
        _pathToMovieReleaseService = pathToMovieReleaseService;
        _songService = songService;
        _movieRepository = movieRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> SaveFromFolderAsync(DiscFolder movieReleaseFolder, MovieId movieId, CancellationToken cancellationToken = default)
    {
        var movieReleaseResult = await _pathToMovieReleaseService
            .GetEntityAsync(movieReleaseFolder.Path)
            .ConfigureAwait(false);

        if (movieReleaseResult.IsFailure)
        {
            return movieReleaseResult;
        }

        var movieRelease = movieReleaseResult.Value;
        var movie = await _movieRepository.GetByIdWithMoviesReleasesAsync(movieId, cancellationToken);
        if (movie is null)
        {
            return Result.Failure(new Error("Movie with passed id is not exists in database."));
        }

        var addingResult = movie.AddRelease(movieRelease);
        if (addingResult.IsFailure)
        {
            return addingResult;
        }

        foreach (var coverPath in movieReleaseFolder.CoversPaths)
        {
            movieRelease.AddCover(coverPath);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var songFile in movieReleaseFolder.Songs)
        {
            var result = await _songService.SaveFromFileInMovieReleaseAsync(songFile, movieRelease.Id, cancellationToken);

            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
        }

        return Result.Success();
    }
}
