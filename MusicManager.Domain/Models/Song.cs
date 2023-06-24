using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Song 
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongId Id { get; private set; }

    public MovieId? MovieId { get; private set; }

    public SongFileInfo? SongFileInfo { get; private set; }

    public TimeSpan Duration { get; private set; }

    public string Name { get; private set; } = string.Empty;

    #endregion

    #region --Constructors--

    public Song(string name)
    {
        Id = SongId.Create();
        Name = name;
        Duration = TimeSpan.Zero;
    }

    #endregion

    #region --Methods--



    #endregion
}

public record SongId(Guid Value)
{
    public static SongId Create() => new(Guid.NewGuid());
}
