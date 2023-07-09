namespace MusicManager.Services.Contracts;

public interface ISongwriterFolder
{
    string Path { get; }

    IReadOnlyCollection<IMovieFolder> MoviesFolders { get; }

    IReadOnlyCollection<ICompilationFolder> CompilationsFolders { get; }
}
