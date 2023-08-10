namespace MusicManager.Domain.Services;

public record CueSheetInfo(string ExecutableFileName, IEnumerable<CueFileTrack> Tracks);
