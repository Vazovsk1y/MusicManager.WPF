using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Song : IAggregateRoot
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongId Id { get; private set; }

    public DiscId DiscId { get; private set; }

    public DiscNumber? DiscNumber { get; private set; }

    public PlaybackInfo PlaybackInfo { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int Order { get; private set; }

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

    private static Result<Song> Create(
        DiscId discId,
        string name,
        int order)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Song>(DomainErrors.NullOrEmptyStringPassed(nameof(name)));
        }

        if (int.IsNegative(order))
        {
            return Result.Failure<Song>(new("Song order must be non-negative number."));
        }

        return new Song(name, discId)
        {
            Order = order
        };
    }

    private static Result<Song> Create(
        DiscId discId, 
        string name,
        int order,
        int discNumber)
    {
        var songCreationResult = Create(discId, name, order);
        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        var discNumberCreationResult = DiscNumber.Create(discNumber);
        if (discNumberCreationResult.IsFailure)
        {
            return Result.Failure<Song>(discNumberCreationResult.Error);
        }

        song.DiscNumber = discNumberCreationResult.Value;
        return song;
    }

    public static Result<Song> Create(
        DiscId discId, 
        string name,
        int order,
        int discNumber, 
        string songFileFullPath,
        TimeSpan songDuration)
    {
        var songCreationResult = Create(discId, name, order, discNumber);

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
        int order,
        string songFileFullPath,
        TimeSpan songDuration)
    {
        var songCreationResult = Create(discId, name, order);

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
        int order,
        int discNumber,
        string songFileFullPath,
        TimeSpan songDuration,
        string cueFileFullPath,
        TimeSpan index00,
        TimeSpan index01)
    {
        var songCreationResult = Create(discId, name, order, discNumber);

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
        int order,
        string songFileFullPath,
        TimeSpan songDuration,
        string cueFileFullPath,
        TimeSpan index00,
        TimeSpan index01)
    {
        var songCreationResult = Create(discId, name, order);

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

    public Result SetDiscNumber(DiscNumber discNumber)
    {
        if (discNumber is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed(nameof(discNumber)));
        }

        DiscNumber = discNumber;
        return Result.Success();
    }

    #endregion
}

public record SongId(Guid Value)
{
    public static SongId Create() => new(Guid.NewGuid());
}
