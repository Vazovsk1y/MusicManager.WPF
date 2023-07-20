using MusicManager.Domain.Shared;

namespace MusicManager.Services.Contracts.Factories;

public interface ISongFileFactory
{
    Result<SongFile> Create(string songFilePath, string? cueFilePath = null);
}


