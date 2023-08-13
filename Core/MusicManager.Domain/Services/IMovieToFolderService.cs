using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IMovieToFolderService
{
    Task<Result<string>> CreateAssociatedFolderAndFileAsync(Movie movie, Songwriter parent);
}