using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts;

public record SongwriterFolder(
	string Path, 
	IEnumerable<MovieFolder> MoviesFolders, 
	IEnumerable<DiscFolder> CompilationsFolders);

