namespace MusicManager.Services.Contracts
{
    public interface IMovieFolder
    {
        string Path { get; }

        IEnumerable<IMovieReleaseFolder> MoviesReleasesFolders { get; }
    }
}
