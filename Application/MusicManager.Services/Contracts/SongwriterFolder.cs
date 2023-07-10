using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts;

public record SongwriterFolder(string Path, IReadOnlyCollection<MovieFolder> MoviesFolders, IReadOnlyCollection<DiscFolder> CompilationsFolders);

