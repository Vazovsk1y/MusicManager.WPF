using MusicManager.Domain.Common;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;

namespace MusicManager.Services.Implementations;

public class SongService : ISongService
{
    private readonly ICompilationRepository _compilationRepository;
    private readonly IMovieReleaseRepository _movieReleaseRepository;
    private readonly IPathToSongService _pathToSongService;
    private readonly IUnitOfWork _unitOfWork;

    public SongService(
        ICompilationRepository compilationRepository,
        IMovieReleaseRepository movieReleaseRepository,
        IUnitOfWork unitOfWork,
        IPathToSongService pathToSongService)
    {
        _compilationRepository = compilationRepository;
        _movieReleaseRepository = movieReleaseRepository;
        _unitOfWork = unitOfWork;
        _pathToSongService = pathToSongService;
    }

    public async Task<Result> SaveFromFileInCompilationAsync(SongFile songFile, DiscId discId, CancellationToken cancellationToken = default)
    {
        var compilation = await _compilationRepository.GetByIdAsync(discId, cancellationToken);
        if (compilation is null) 
        {
            return Result.Failure(new Error("Compilation with passed id is not exists in database."));
        }

        return songFile.CueFilePath is null ? 
            await SaveFromSingeFile(songFile.SongFilePath, compilation, cancellationToken)
            :
            await SaveFromCue(songFile.SongFilePath, songFile.CueFilePath, compilation, cancellationToken);
    }

    public async Task<Result> SaveFromFileInMovieReleaseAsync(SongFile songFile, DiscId discId, CancellationToken cancellationToken = default)
    {
        var movieRelease = await _movieReleaseRepository.GetByIdAsync(discId, cancellationToken);
        if (movieRelease is null)
        {
            return Result.Failure(new Error("MovieRelease with passed id is not exists in database."));
        }

        return songFile.CueFilePath is null ?
            await SaveFromSingeFile(songFile.SongFilePath, movieRelease, cancellationToken)
            :
            await SaveFromCue(songFile.SongFilePath, songFile.CueFilePath, movieRelease, cancellationToken);
    }

    private async Task<Result> SaveFromCue(string songFilePath, string cueFilePath, Disc disc, CancellationToken cancellationToken)
    {
        var songsResult = await _pathToSongService.GetEntitiesFromCueFileAsync(songFilePath, cueFilePath, disc.Id);

        if (songsResult.IsFailure)
        {
            return songsResult;
        }

        foreach(var song in songsResult.Value)
        {
            var result = disc.AddSong(song);
            if (result.IsFailure)
            {
                return result;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> SaveFromSingeFile(string songFilePath, Disc disc, CancellationToken cancellationToken)
    {
        var songResult = await _pathToSongService.GetEntityAsync(songFilePath, disc.Id);

        if (songResult.IsFailure)
        {
            return songResult;
        }

        var result = disc.AddSong(songResult.Value);
        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
