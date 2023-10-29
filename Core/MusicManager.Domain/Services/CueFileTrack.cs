using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Services;

public class CueFileTrack
{
    public string Title { get; set; } = ProductionInfo.UndefinedCountry;

    public string Performer { get; set; } = ProductionInfo.UndefinedCountry;

	public int TrackOrder { get; set; }

    public string? Isrc { get; set; }

    public TimeSpan Index00 { get; set; }

    public TimeSpan Index01 { get; set; }
}
