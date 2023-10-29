using MusicManager.Domain.Shared;

namespace MusicManager.Services.Contracts.Factories;

public interface ISongFileFactory
{
    Result<SongFile> Create(FileInfo songFileInfo);
}


