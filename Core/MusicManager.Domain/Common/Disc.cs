using MusicManager.Domain.Entities;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Common;

public class Disc : IAggregateRoot
{
    #region --Fields--

    protected readonly List<Song> _songs = new();

    protected readonly List<Cover> _covers = new();

    #endregion

    #region --Properties--

    public DiscId Id { get; init; }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; protected set; }

    public ProductionInfo ProductionInfo { get; protected set; } = null!;

    public DiscType Type { get; protected set; }

    public string Identifier { get; protected set; } = string.Empty;

    public IReadOnlyCollection<Song> Songs => _songs.ToList();

    public IReadOnlyCollection<Cover> Covers => _covers.ToList();

    #endregion

    #region --Constructors--

    protected Disc()
    {
        Id = DiscId.Create();
    }

    #endregion

    #region --Methods--

    public virtual Result AddCover(string coverPath)
    {
        var coverCreationResult = Cover.Create(Id, coverPath);
        if (coverCreationResult.IsFailure)
        {
            return Result.Failure(new (coverCreationResult.Error));
        }

        if (_covers.SingleOrDefault(e => 
        e.FullPath == coverCreationResult.Value.FullPath) is not null)
        {
            return Result.Failure(new Error($"Cover with passed path [{coverPath}] is already exists."));
        }

        _covers.Add(coverCreationResult.Value);
        return Result.Success();
    }

    public virtual Result AddSong(Song song, bool checkPlaybackInfo = false)
    {
        if (song is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed(nameof(song)));
        }

        if (_songs.SingleOrDefault(i => i.Id == song.Id) is not null)
        {
            return Result.Failure(DomainErrors.EntityAlreadyExists(nameof(song)));
        }

        if (checkPlaybackInfo && _songs.SingleOrDefault(e =>
        e.PlaybackInfo == song.PlaybackInfo) is not null)
        {
            return Result.Failure(new Error("Song with passed playback info is already exists."));
        }

        _songs.Add(song);
        return Result.Success();
    }

    public virtual Result SetDirectoryInfo(string fullPath)
    {
        var result = EntityDirectoryInfo.Create(fullPath);

        if (result.IsSuccess)
        {
            EntityDirectoryInfo = result.Value;
            return result;
        }

        return result;
    }

    public virtual Result SetIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure(DomainErrors.NullOrEmptyStringPassed(nameof(identifier)));
        }

        Identifier = identifier;
        return Result.Success();
    }

    public virtual Result RemoveSong(SongId songId)
    {
        var song = _songs.SingleOrDefault(e => e.Id == songId);
        if (song is null)
        {
            return Result.Failure(new Error("Unable to remove song because it with this id is not exists."));
        }

        _songs.Remove(song);
        return Result.Success();
    }

    public virtual Result SetProductionInfo(string? productionCountry, int? productionYear)
    {
        if (productionYear is null && Type != DiscType.Bootleg)
        {
            return Result.Failure(new Error("At least production year must be setted."));
        }

        var result = ProductionInfo.Create(productionCountry, productionYear);

        if (result.IsSuccess)
        {
            ProductionInfo = result.Value;
            return Result.Success();
        }

        return Result.Failure(result.Error);
    }
    public virtual Result SetDiscType(DiscType discType)
    {
        if (discType == null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed("disc type"));
        }

        if (discType == DiscType.Bootleg)
        {
            ProductionInfo = ProductionInfo.None;
        }

        Type = discType;
        return Result.Success();
    }

    #endregion
}

public record DiscId(Guid Value)
{
    public static DiscId Create() => new(Guid.NewGuid());
}
