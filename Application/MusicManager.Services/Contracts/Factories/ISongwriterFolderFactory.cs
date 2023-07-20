using MusicManager.Domain.Shared;

namespace MusicManager.Services.Contracts.Factories;

public interface ISongwriterFolderFactory
{
    Result<SongwriterFolder> Create(string songWriterPath);
}


