using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Models;

public class Song : IAggregateRoot
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongId Id { get; private set; }

    public DiscId DiscId { get; private set; }

    public string? DiscNumber { get; private set; }

    public PlaybackInfo? PlaybackInfo { get; private set; }

    public string Name { get; private set; } = string.Empty;

    #endregion

    #region --Constructors--

    private Song(string name, DiscId discId)
    {
        Id = SongId.Create();
        DiscId = discId;
        Name = name;
    }

    #endregion

    #region --Methods--

    public static Result<Song> Create(
        DiscId discId,
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Song>(DomainErrors.NullOrEmptyStringPassed(nameof(name)));
        }

        return new Song(name, discId);
    }

    public static Result<Song> Create(
        DiscId discId, 
        string name, 
        string discNumber)
    {
        if (string.IsNullOrEmpty(discNumber))
        {
            return Result.Failure<Song>(DomainErrors.NullOrEmptyStringPassed(nameof(discNumber)));
        }

        var songCreationResult = Create(discId, name);
        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        song.DiscNumber = discNumber;
        return song;
    }

    public static Result<Song> Create(
        DiscId discId, 
        string name, 
        string discNumber, 
        string songFileFullPath,
        TimeSpan songDuration)
    {
        var songCreationResult = Create(discId, name, discNumber);

        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        var settingInfoResult = song.SetPlaybackInfo(songFileFullPath, songDuration);

        if (settingInfoResult.IsFailure)
        {
            return Result.Failure<Song>(settingInfoResult.Error);
        }

        return song;
    }

    public static Result<Song> Create(
        DiscId discId,
        string name,
        string songFileFullPath,
        TimeSpan songDuration)
    {
        var songCreationResult = Create(discId, name);

        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        var settingInfoResult = song.SetPlaybackInfo(songFileFullPath, songDuration);

        if (settingInfoResult.IsFailure)
        {
            return Result.Failure<Song>(settingInfoResult.Error);
        }

        return song;
    }

    public static Result<Song> Create(
        DiscId discId, 
        string name,
        string discNumber,
        string songFileFullPath,
        TimeSpan songDuration,
        string cueFileFullPath,
        TimeSpan index00,
        TimeSpan index01)
    {
        var songCreationResult = Create(discId, name, discNumber);

        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        var settingPlayInfoResult = song.SetPlaybackInfo(songFileFullPath, songDuration, cueFileFullPath, index00, index01);

        if (settingPlayInfoResult.IsFailure)
        {
            return Result.Failure<Song>(settingPlayInfoResult.Error);
        }

        return song;
    }

    public static Result<Song> Create(
        DiscId discId, 
        string name,
        string songFileFullPath,
        TimeSpan songDuration,
        string cueFileFullPath,
        TimeSpan index00,
        TimeSpan index01)
    {
        var songCreationResult = Create(discId, name);

        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        var settingPlayInfoResult = song.SetPlaybackInfo(songFileFullPath, songDuration, cueFileFullPath, index00, index01);

        if (settingPlayInfoResult.IsFailure)
        {
            return Result.Failure<Song>(settingPlayInfoResult.Error);
        }

        return song;
    }

    public Result SetPlaybackInfo(
        string fullPath, 
        TimeSpan duration)
    {
        var settingSongFileInfoResult = PlaybackInfo.Create(fullPath, Id, duration);

        if (settingSongFileInfoResult.IsFailure)
        {
            return Result.Failure(settingSongFileInfoResult.Error);
        }

        PlaybackInfo = settingSongFileInfoResult.Value;
        return Result.Success();
    }

    public Result SetPlaybackInfo(
        string fullPath, 
        TimeSpan duration, 
        string cueFileFullPath,
        TimeSpan index00,
        TimeSpan index01)
    {
        var settingSongFileInfoResult = PlaybackInfo.Create(fullPath, Id, duration, cueFileFullPath, index00, index01);

        if (settingSongFileInfoResult.IsFailure)
        {
            return Result.Failure(settingSongFileInfoResult.Error);
        }
        PlaybackInfo = settingSongFileInfoResult.Value;
        return Result.Success();
    }

    #endregion
}

public record SongId(Guid Value)
{
    public static SongId Create() => new(Guid.NewGuid());
}
