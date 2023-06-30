using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IPathToMovieService
{
    Task<Result<Movie>> GetEntityAsync(string moviePath, SongwriterId songwriterId);
}
