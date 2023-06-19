using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Song : Entity
{
    public Song(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public SongFileInfo? SongFileInfo { get; private set; }

    public TimeSpan Duration { get; private set; }

    public string Name { get; private set; }

    public Guid? MovieId { get; private set; }
}
