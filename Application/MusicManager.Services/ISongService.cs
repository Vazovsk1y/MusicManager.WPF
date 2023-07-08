using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface ISongService
    {
        Task<Result> SaveFromFolder(ISongFile songFile, DiscId discId);
    }
}