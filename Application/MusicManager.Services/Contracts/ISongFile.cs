namespace MusicManager.Services.Contracts;

public interface ISongFile
{
    string SongFilePath { get; }

    string? CueFilePath { get; }
}
