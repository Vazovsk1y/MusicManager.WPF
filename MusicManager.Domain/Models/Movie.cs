using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Movie : Entity
{
    private readonly List<Disc> _discs = new();

    public Movie(
        Guid id, 
        string title, 
        ProductionInfo productionInfo, 
        DirectorInfo directorInfo) : base(id)
    {
        Title = title;
        ProductionInfo = productionInfo;
        DirectorInfo = directorInfo;
    }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public Guid SongwriterId { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public DirectorInfo DirectorInfo { get; private set; }

    public string Title { get; private set; }

    public IReadOnlyCollection<Disc> Discs => _discs.ToList();
}
