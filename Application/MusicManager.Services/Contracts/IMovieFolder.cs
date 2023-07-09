namespace MusicManager.Services.Contracts
{
    public interface IMovieFolder
    {
        string Path { get; }

        IReadOnlyCollection<IMovieReleaseFolder> MoviesReleasesFolders { get; }
    }
}
