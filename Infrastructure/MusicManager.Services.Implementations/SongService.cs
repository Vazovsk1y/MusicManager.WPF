using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Services.Implementations;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class SongService : ISongService
{
    private readonly IFileToSongService _pathToSongService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ISongToFileService _songToFile;

    public SongService(
        IApplicationDbContext dbContext,
        IFileToSongService pathToSongService,
        ISongToFileService songToFileService)
    {
        _dbContext = dbContext;
        _pathToSongService = pathToSongService;
        _songToFile = songToFileService;
    }

	public async Task<Result> DeleteAsync(SongId songId, CancellationToken cancellationToken = default)
	{
        var song = await _dbContext.Songs
            .SingleOrDefaultAsync(e => e.Id == songId, cancellationToken);

        if (song is null)
        {
            return Result.Failure(ServicesErrors.SongWithPassedIdIsNotExists());
        }

        _dbContext.Songs.Remove(song);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
	}

	public async Task<Result<IReadOnlyCollection<SongDTO>>> GetAllAsync(DiscId discId, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext
            .Songs
            .Include(e => e.PlaybackInfo)
            .AsNoTracking()
            .Where(e => e.DiscId == discId)
            .Select(e => e.ToDTO())
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Result<IReadOnlyCollection<SongDTO>>> SaveAsync(SongAddDTO songAddDTO, bool moveToParentFolder = true, CancellationToken cancellationToken = default)
    {
        var disc = await _dbContext
           .Discs
           .Include(e => e.Songs)
           .ThenInclude(e => e.PlaybackInfo)
           .SingleOrDefaultAsync(e => e.Id == songAddDTO.DiscId, cancellationToken);

        if (disc is null)
        {
            return Result.Failure<IReadOnlyCollection<SongDTO>>(ServicesErrors.DiscWithPassedIdIsNotExists());
        }

        if (songAddDTO.SongFile.IsCueFile)
        {
            var result = await AddToDiscFromCue(songAddDTO.SongFile.SongFilePath, disc, false, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<IReadOnlyCollection<SongDTO>>(result.Error);
            }

            var songs = result.Value;
            if (moveToParentFolder)
            {
                var anySong = songs.First();
                var movingCueFileResult = await _songToFile.CopyToAsync(anySong.PlaybackInfo.CueInfo!.CueFilePath, disc, songAddDTO.DiscNumber);
                if (movingCueFileResult.IsFailure)
                {
                    return Result.Failure<IReadOnlyCollection<SongDTO>>(movingCueFileResult.Error);
                }

                var movingExecutableFileResult = await _songToFile.CopyToAsync(anySong.PlaybackInfo.ExecutableFilePath, disc, songAddDTO.DiscNumber);
                if (movingExecutableFileResult.IsFailure)
                {
                    return Result.Failure<IReadOnlyCollection<SongDTO>>(movingExecutableFileResult.Error);
                }

                foreach (var item in songs)
                {
                    item.SetPlaybackInfo(
                        movingExecutableFileResult.Value,
                        item.PlaybackInfo.Duration,
                        movingCueFileResult.Value,
                        item.PlaybackInfo.CueInfo.Index00,
                        item.PlaybackInfo.CueInfo.Index01,
                        item.PlaybackInfo.CueInfo.SongTitleInCue);
                }
            }

            foreach (var song in songs)
            {
                song.SetDiscNumber(songAddDTO.DiscNumber);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return songs.Select(e => e.ToDTO()).ToList();
        }
        else
        {
            var result = await AddToDiscFromSingeFile(songAddDTO.SongFile.SongFilePath, disc, false, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<IReadOnlyCollection<SongDTO>>(result.Error);
            }

            var song = result.Value;
            if (moveToParentFolder)
            {
                var movingExecutableFileResult = await _songToFile.CopyToAsync(song.PlaybackInfo.ExecutableFilePath, disc, songAddDTO.DiscNumber);
                if (movingExecutableFileResult.IsFailure)
                {
                    return Result.Failure<IReadOnlyCollection<SongDTO>>(movingExecutableFileResult.Error);
                }
                song.SetPlaybackInfo(
                movingExecutableFileResult.Value,
                song.PlaybackInfo.Duration);
            }
            
            song.SetDiscNumber(songAddDTO.DiscNumber);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new List<SongDTO> { song.ToDTO() };
        }
    }

    public async Task<Result<IReadOnlyCollection<SongDTO>>> SaveFromFileAsync(SongFile songFile, DiscId discId, bool ignoreSongAddingResult, CancellationToken cancellationToken = default)
    {
        var disc = await _dbContext
            .Discs
            .Include(e => e.Songs)
            .ThenInclude(e => e.PlaybackInfo)
            .SingleOrDefaultAsync(e => e.Id == discId, cancellationToken);

        if (disc is null)
        {
            return Result.Failure<IReadOnlyCollection<SongDTO>>(ServicesErrors.DiscWithPassedIdIsNotExists());
        }

        if (songFile.IsCueFile)
        {
            var result = await AddToDiscFromCue(songFile.SongFilePath, disc, ignoreSongAddingResult, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<IReadOnlyCollection<SongDTO>>(result.Error);
            }

            var songs = result.Value;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return songs.Select(e => e.ToDTO()).ToList();
        }
        else
        {
            var result = await AddToDiscFromSingeFile(songFile.SongFilePath, disc, ignoreSongAddingResult, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<IReadOnlyCollection<SongDTO>>(result.Error);
            }

            var song = result.Value;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new List<SongDTO> { song.ToDTO() };
        }
    }

    public async Task<Result> UpdateAsync(SongUpdateDTO songUpdateDTO, CancellationToken cancellationToken = default)
    {
        var song = await _dbContext.Songs
            .Include(e => e.PlaybackInfo)
            .SingleOrDefaultAsync(e => e.Id == songUpdateDTO.SongId, cancellationToken);

        if (song is null)
        {
            return Result.Failure(ServicesErrors.SongWithPassedIdIsNotExists());
        }

        var updateActions = new List<Result>()
        {
            song.SetTitle(songUpdateDTO.Title),
            song.SetOrder(songUpdateDTO.Order)
        };

        if (updateActions.Any(e => e.IsFailure))
        {
            return Result.Failure(new(string.Join("\n", updateActions.Where(e => e.IsFailure).Select(e => e.Error.Message))));
        }

        var fileUpdatingResult = await _songToFile.UpdateAsync(song, cancellationToken);
        if (fileUpdatingResult.IsSuccess)
        {
            song.SetPlaybackInfo(fileUpdatingResult.Value, song.PlaybackInfo.Duration);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result<IEnumerable<Song>>> AddToDiscFromCue(string cueFilePath, Disc disc, bool ignoreAddingResult, CancellationToken cancellationToken)
    {
        var songsResult = await _pathToSongService.GetEntitiesFromCueFileAsync(cueFilePath, disc.Id);

        if (songsResult.IsFailure)
        {
            return Result.Failure<IEnumerable<Song>>(songsResult.Error);
        }

        var songs = songsResult.Value;
        foreach (var song in songs)
        {
            var addingResult = disc.AddSong(song);
            if (addingResult.IsFailure && !ignoreAddingResult)
            {
                return Result.Failure<IEnumerable<Song>>(addingResult.Error);
            }
        }

        return Result.Success(songs);
    }

    private async Task<Result<Song>> AddToDiscFromSingeFile(string songFilePath, Disc disc, bool ignoreAddingResult, CancellationToken cancellationToken)
    {
        var songResult = await _pathToSongService.GetEntityAsync(songFilePath, disc.Id);

        if (songResult.IsFailure)
        {
            return Result.Failure<Song>(songResult.Error);
        }

        var song = songResult.Value;
        var addingResult = disc.AddSong(song);
        if (addingResult.IsFailure && !ignoreAddingResult)
        {
            return Result.Failure<Song>(addingResult.Error);
        }

        return song;
    }
}
