using MusicManager.Domain.Common;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Common;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Mappers;

namespace MusicManager.Services.Implementations;

public class SongService : ISongService
{
    private readonly IBaseDiscRepository<Disc> _discRepository;
    private readonly IPathToSongService _pathToSongService;
    private readonly IUnitOfWork _unitOfWork;

    public SongService(
        IUnitOfWork unitOfWork,
        IPathToSongService pathToSongService,
        IBaseDiscRepository<Disc> discRepository)
    {
        _unitOfWork = unitOfWork;
        _pathToSongService = pathToSongService;
        _discRepository = discRepository;
    }

    public async Task<Result<IEnumerable<SongDTO>>> GetAllAsync(DiscId discId, CancellationToken cancellationToken = default)
    {
        var disc = await _discRepository.LoadByIdWithSongsAsync(discId, cancellationToken);

        return disc is null ?
            Result.Failure<IEnumerable<SongDTO>>(new Error("Disc with passed id is not exists."))
            :
            disc.Songs.Select(e => e.ToDTO()).ToList();
    }

    public async Task<Result> SaveFromFileAsync(SongFile songFile, DiscId discId, CancellationToken cancellationToken = default)
    {
        var disc = await _discRepository.LoadByIdWithSongsAsync(discId, cancellationToken);
        if (disc is null)
        {
            return Result.Failure(new Error("Disc with passed id is not exists in database."));
        }

        return songFile.CueFilePath is null ?
            await SaveFromSingeFile(songFile.SongFilePath, disc, cancellationToken)
            :
            await SaveFromCue(songFile.SongFilePath, songFile.CueFilePath, disc, cancellationToken);
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
