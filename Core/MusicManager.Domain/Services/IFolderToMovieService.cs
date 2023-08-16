using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IFolderToMovieService
{
    Task<Result<Movie>> GetEntityAsync(string moviePath, SongwriterId songwriterId);
}
