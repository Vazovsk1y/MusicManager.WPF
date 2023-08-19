﻿using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
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

    public async Task<Result<IEnumerable<SongDTO>>> SaveAsync(SongAddDTO songAddDTO, CancellationToken cancellationToken = default)
    {
        var disc = await _dbContext
           .Discs
           .Include(e => e.Songs)
           .ThenInclude(e => e.PlaybackInfo)
           .SingleOrDefaultAsync(e => e.Id == songAddDTO.DiscId, cancellationToken);

        if (disc is null)
        {
            return Result.Failure<IEnumerable<SongDTO>>(ServicesErrors.DiscWithPassedIdIsNotExists());
        }

        if (songAddDTO.SongFile.IsCueFile)
        {
            var result = await AddToDiscFromCue(songAddDTO.SongFile.SongFilePath, disc, false, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<IEnumerable<SongDTO>>(result.Error);
            }

            var songs = result.Value;
            var anySong = songs.First();
            var movingCueFileResult = await _songToFile.CopyToAsync(anySong.PlaybackInfo!.CueInfo!.CueFilePath, disc, songAddDTO.DiscNumber);
            if (movingCueFileResult.IsFailure)
            {
                return Result.Failure<IEnumerable<SongDTO>>(movingCueFileResult.Error);
            }

            var movingExecutableFileResult = await _songToFile.CopyToAsync(anySong.PlaybackInfo.ExecutableFileFullPath, disc, songAddDTO.DiscNumber);
            if (movingExecutableFileResult.IsFailure)
            {
                return Result.Failure<IEnumerable<SongDTO>>(movingExecutableFileResult.Error);
            }

            foreach (var item in songs)
            {
                item.SetDiscNumber(songAddDTO.DiscNumber);
                item.SetPlaybackInfo(
                    movingExecutableFileResult.Value, 
                    item.PlaybackInfo.SongDuration, 
                    movingCueFileResult.Value, 
                    item.PlaybackInfo.CueInfo.Index00, 
                    item.PlaybackInfo.CueInfo.Index01);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return songs.Select(e => e.ToDTO()).ToList();
        }
        else
        {
            var result = await AddToDiscFromSingeFile(songAddDTO.SongFile.SongFilePath, disc, false, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<IEnumerable<SongDTO>>(result.Error);
            }

            var song = result.Value;
            var movingExecutableFileResult = await _songToFile.CopyToAsync(song.PlaybackInfo.ExecutableFileFullPath, disc, songAddDTO.DiscNumber);
            if (movingExecutableFileResult.IsFailure)
            {
                return Result.Failure<IEnumerable<SongDTO>>(movingExecutableFileResult.Error);
            }

            song.SetDiscNumber(songAddDTO.DiscNumber);
            song.SetPlaybackInfo(
                movingExecutableFileResult.Value, 
                song.PlaybackInfo.SongDuration);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new List<SongDTO> { song.ToDTO() };
        }
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

        if (songFile.IsCueFile)
        {
            var result = await AddToDiscFromCue(songFile.SongFilePath, disc, ignoreSongAddingResult, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<IEnumerable<SongDTO>>(result.Error);
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
                return Result.Failure<IEnumerable<SongDTO>>(result.Error);
            }

            var song = result.Value;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new List<SongDTO> { song.ToDTO() };
        }
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
            var addingResult = disc.AddSong(song, true);
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
        var addingResult = disc.AddSong(song, true);
        if (addingResult.IsFailure && !ignoreAddingResult)
        {
            return Result.Failure<Song>(addingResult.Error);
        }

        return song;
    }
}
