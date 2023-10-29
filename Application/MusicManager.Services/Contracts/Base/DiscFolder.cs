namespace MusicManager.Services.Contracts.Base;

public record DiscFolder(
	string Path, 
	IEnumerable<SongFile> SongsFiles, 
	IEnumerable<string> CoversPaths, 
	string? LinkPath = null);
