using MusicManager.Domain.Shared;

namespace MusicManager.Services.Contracts.Factories;

public interface IMovieFolderFactory
{
    Result<MovieFolder> Create(string moviePath);
}


