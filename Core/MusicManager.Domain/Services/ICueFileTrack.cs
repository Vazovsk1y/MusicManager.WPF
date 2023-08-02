namespace MusicManager.Domain.Services;

public interface ICueFileTrack
{
    public string Title { get; set; } 

    public string Performer { get; set; }

    public int TrackPosition { get; set; }

    public string? Isrc { get; set; }

    public TimeSpan Index00 { get; set; }

    public TimeSpan Index01 { get; set; }
}
