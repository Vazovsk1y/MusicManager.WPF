using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface ISongwriterService
    {
        Task<Result> SaveFromFolder(ISongwriterFolder songwriterFolder);
    }
}