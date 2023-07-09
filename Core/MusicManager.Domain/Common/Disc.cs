using MusicManager.Domain.Entities;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Common;

public abstract class Disc : IAggregateRoot
{
    #region --Fields--

    protected readonly List<Song> _songs = new();

    protected readonly List<Cover> _covers = new();

    #endregion

    #region --Properties--

    public DiscId Id { get; protected set; }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; protected set; }

    public ProductionInfo ProductionInfo { get; protected set; }

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



    #endregion
}

public record DiscId(Guid Value)
{
    public static DiscId Create() => new(Guid.NewGuid());
}
