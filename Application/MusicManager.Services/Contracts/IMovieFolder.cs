using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts
{
    public interface IMovieFolder
    {
        string Path { get; }

        IReadOnlyCollection<IDiscFolder> MoviesReleasesFolders { get; }
    }
}
