using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class SongService : ISongService
{
    private readonly IPathToSongService _pathToSongService;
    private readonly IApplicationDbContext _dbContext;

    public SongService(
        IApplicationDbContext dbContext,
        IPathToSongService pathToSongService)
    {
        _dbContext = dbContext;
        _pathToSongService = pathToSongService;
    }

    public async Task<Result<IEnumerable<SongDTO>>> GetAllAsync(DiscId discId, CancellationToken cancellationToken = default)
    {
        var disc = await _dbContext
            .Discs
            .AsNoTracking()
            .Include(e => e.Songs)
            .ThenInclude(e => e.PlaybackInfo)
            .SingleOrDefaultAsync(e => e.Id == discId, cancellationToken);

        return disc is null ?
            Result.Failure<IEnumerable<SongDTO>>(ServicesErrors.DiscWithPassedIdIsNotExists())
            :
            disc.Songs.Select(e => e.ToDTO()).ToList();
    }

    public async Task<Result<IEnumerable<SongDTO>>> SaveFromFileAsync(SongFile songFile, DiscId discId, bool ignoreSongAddingResult, CancellationToken cancellationToken = default)
    {
        var disc = await _dbContext
            .Discs
            .Include(e => e.Songs)
            .ThenInclude(e => e.PlaybackInfo)
            .SingleOrDefaultAsync(e => e.Id == discId, cancellationToken);

        if (disc is null)
        {
            return Result.Failure<IEnumerable<SongDTO>>(ServicesErrors.DiscWithPassedIdIsNotExists());
        }

        if (songFile.CueFilePath is null)
        {
            var result = await SaveFromSingeFile(songFile.SongFilePath, disc, ignoreSongAddingResult, cancellationToken);
            return result.IsFailure ? Result.Failure<IEnumerable<SongDTO>>(result.Error) : new List<SongDTO>() { result.Value };
        }
        else
        {
            return await SaveFromCue(songFile.CueFilePath, disc, ignoreSongAddingResult, cancellationToken);
        }
    }

    private async Task<Result<IEnumerable<SongDTO>>> SaveFromCue(string cueFilePath, Disc disc, bool ignoreAddingResult, CancellationToken cancellationToken)
    {
        var songsResult = await _pathToSongService.GetEntitiesFromCueFileAsync(cueFilePath, disc.Id);

        if (songsResult.IsFailure)
        {
            return Result.Failure<IEnumerable<SongDTO>>(songsResult.Error);
        }

        var songs = songsResult.Value;
        foreach (var song in songs)
        {
            var addingResult = disc.AddSong(song, true);
            if (addingResult.IsFailure && !ignoreAddingResult)
            {
                return Result.Failure<IEnumerable<SongDTO>>(addingResult.Error);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return songs.Select(s => s.ToDTO()).ToList();
    }

    private async Task<Result<SongDTO>> SaveFromSingeFile(string songFilePath, Disc disc, bool ignoreAddingResult, CancellationToken cancellationToken)
    {
        var songResult = await _pathToSongService.GetEntityAsync(songFilePath, disc.Id);

        if (songResult.IsFailure)
        {
            return Result.Failure<SongDTO>(songResult.Error);
        }

        var song = songResult.Value;
        var addingResult = disc.AddSong(song, true);
        if (addingResult.IsFailure && !ignoreAddingResult)
        {
            return Result.Failure<SongDTO>(addingResult.Error);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return song.ToDTO();
    }
}
