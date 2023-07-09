using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts;

public interface ISongwriterFolder
{
    string Path { get; }

    IReadOnlyCollection<IMovieFolder> MoviesFolders { get; }

    IReadOnlyCollection<IDiscFolder> CompilationsFolders { get; }
}
