namespace MusicManager.Services.Contracts;

public interface ISongwriterFolder
{
    string Path { get; }

    IEnumerable<IMovieFolder> MoviesFolders { get; }

    IEnumerable<ICompilationFolder> CompilationsFolders { get; }
}
