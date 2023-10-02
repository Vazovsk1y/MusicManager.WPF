using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IMovieToFolderService
{
    Task<Result<string>> CreateAssociatedAsync(Movie movie, Songwriter parent);

    Task<Result<string>> UpdateIfExistsAsync(Movie movie);
}