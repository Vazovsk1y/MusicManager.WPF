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
            Result.Failure<IEnumerable<SongDTO>>(ServicesErrors.DiscWithPassedIdIsNotExists())
            :
            disc.Songs.Select(e => e.ToDTO()).ToList();
    }

    public async Task<Result<IEnumerable<SongDTO>>> SaveFromFileAsync(SongFile songFile, DiscId discId, CancellationToken cancellationToken = default)
    {
        var disc = await _discRepository.LoadByIdWithSongsAsync(discId, cancellationToken);
        if (disc is null)
        {
            return Result.Failure<IEnumerable<SongDTO>>(ServicesErrors.DiscWithPassedIdIsNotExists());
        }

        return songFile.CueFilePath is null ?
            await SaveFromSingeFile(songFile.SongFilePath, disc, cancellationToken)
            :
            await SaveFromCue(songFile.SongFilePath, songFile.CueFilePath, disc, cancellationToken);
    }

    private async Task<Result<IEnumerable<SongDTO>>> SaveFromCue(string songFilePath, string cueFilePath, Disc disc, CancellationToken cancellationToken)
    {
        var songsResult = await _pathToSongService.GetEntitiesFromCueFileAsync(songFilePath, cueFilePath, disc.Id);

        if (songsResult.IsFailure)
        {
            return Result.Failure<IEnumerable<SongDTO>>(songsResult.Error);
        }

        var songs = songsResult.Value;
        foreach (var song in songs)
        {
            var result = disc.AddSong(song);
            if (result.IsFailure)
            {
                return Result.Failure<IEnumerable<SongDTO>>(result.Error);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return songs.Select(s => s.ToDTO()).ToList();
    }

    private async Task<Result<IEnumerable<SongDTO>>> SaveFromSingeFile(string songFilePath, Disc disc, CancellationToken cancellationToken)
    {
        var songResult = await _pathToSongService.GetEntityAsync(songFilePath, disc.Id);

        if (songResult.IsFailure)
        {
            return Result.Failure<IEnumerable<SongDTO>>(songResult.Error);
        }

        var song = songResult.Value;
        var result = disc.AddSong(songResult.Value);
        if (result.IsFailure)
        {
            return Result.Failure<IEnumerable<SongDTO>>(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new List<SongDTO>()
        {
            song.ToDTO(),
        };
    }
}
