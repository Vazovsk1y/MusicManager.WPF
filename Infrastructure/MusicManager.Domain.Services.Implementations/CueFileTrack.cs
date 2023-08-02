namespace MusicManager.Domain.Services.Implementations;

public class CueFileTrack : ICueFileTrack
{
    public string Title { get; set; } = string.Empty;

    public string Performer { get; set; } = string.Empty;

    public int TrackPosition { get; set; }

    public string? Isrc { get; set;}

    public TimeSpan Index00 { get; set; }

    public TimeSpan Index01 { get; set; }
}
