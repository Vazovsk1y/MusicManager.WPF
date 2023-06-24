using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IStorageToMovieService
{
    Task<Result<Movie>> GetEntityAsync(IStorage movieStorage, SongwriterId songwriterId);
}
