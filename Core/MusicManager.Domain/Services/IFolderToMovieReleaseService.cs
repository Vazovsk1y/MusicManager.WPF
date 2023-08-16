using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IFolderToMovieReleaseService
{
    Task<Result<MovieRelease>> GetEntityAsync(string movieReleasePath);
}
