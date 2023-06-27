using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Song 
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongId Id { get; private set; }

    public DiscId DiscId { get; private set; }

    public string? DiscNumber { get; private set; }

    public SongFileInfo? SongFileInfo { get; private set; }

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

    public static Result<Song> Create(string name, DiscId discId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Song>(DomainErrors.NullOrEmptyStringPassedError(nameof(name)));
        }

        return new Song(name, discId);
    }

    public static Result<Song> Create(string name, DiscId discId, string discNumber)
    {
        var songCreationResult = Create(name, discId);

        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        song.DiscNumber = discNumber;
        return song;
    }

    public Result SetSongFileInfo(string fullPath, TimeSpan Duration)
    {
        var settingSongFileInfoResult = SongFileInfo.Create(fullPath, Duration);

        if (settingSongFileInfoResult.IsFailure)
        {
            return Result.Failure(settingSongFileInfoResult.Error);
        }

        SongFileInfo = settingSongFileInfoResult.Value;
        return Result.Success();
    }

    #endregion
}

public record SongId(Guid Value)
{
    public static SongId Create() => new(Guid.NewGuid());
}
