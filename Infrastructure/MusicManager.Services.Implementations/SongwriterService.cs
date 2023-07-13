using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;

namespace MusicManager.Services.Implementations;

public class SongwriterService : ISongwriterService
{
    private readonly ISongwriterRepository _songwriterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPathToSongwriterService _pathToSongwriterService;
    private readonly IMovieService _movieService;
    private readonly ICompilationService _compilationService;

    public SongwriterService(
        ISongwriterRepository songwriterRepository,
        IUnitOfWork unitOfWork,
        IPathToSongwriterService pathToSongwriterService,
        IMovieService movieService,
        ICompilationService compilationService)
    {
        _songwriterRepository = songwriterRepository;
        _unitOfWork = unitOfWork;
        _pathToSongwriterService = pathToSongwriterService;
        _movieService = movieService;
        _compilationService = compilationService;
    }

    public async Task<Result> SaveFromFolderAsync(SongwriterFolder songwriterFolder, CancellationToken cancellationToken = default)
    {
        var songWriterResult = await _pathToSongwriterService
            .GetEntityAsync(songwriterFolder.Path)
            .ConfigureAwait(false);

        if (songWriterResult.IsFailure)
        {
            return songWriterResult;
        }

        var songwriter = songWriterResult.Value;
        if (await _songwriterRepository.IsExistsWithPassedDirectoryInfo(songwriter.EntityDirectoryInfo!))
        {
            return Result.Failure(new Error("Songwriter with passed directory path is already exists."));
        }

        await _songwriterRepository.InsertAsync(songwriter, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var movieFolder in songwriterFolder.MoviesFolders)
        {
            var movieResult = await _movieService.SaveFromFolderAsync(movieFolder, songwriter.Id, cancellationToken);

            if (movieResult.IsFailure)
            {
                return movieResult;
            }
        }

        foreach (var compilationFolder in songwriterFolder.CompilationsFolders)
        {
            var compilationResult = await _compilationService.SaveFromFolderAsync(compilationFolder, songwriter.Id, cancellationToken);

            if (compilationResult.IsFailure)
            {
                return compilationResult;
            }
        }

        return Result.Success();
    }
}


