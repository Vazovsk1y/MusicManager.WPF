using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Disc : Entity
{
    private readonly List<Song> _songs = new();

    public Disc(
        Guid id, 
        ProductionInfo productionInfo,
        DiscType discType,
        string identifier) : base(id)
    {
        ProductionInfo = productionInfo;
        DiscType = discType;
        Identifier = identifier;
    }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public Guid? MovieId { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public DiscType DiscType { get; private set; }

    public string Identifier { get; private set; }

    public IReadOnlyCollection<Song> Songs => _songs.ToList();
}
