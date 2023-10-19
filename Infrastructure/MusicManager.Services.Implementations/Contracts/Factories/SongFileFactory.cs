using MusicManager.Domain.Constants;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Factories;
using System.IO;

namespace MusicManager.Services.Implementations.Contracts.Factories;

public class SongFileFactory : ISongFileFactory
{
    public Result<SongFile> Create(FileInfo songFilePath)
    {
        if (!songFilePath.Exists)
        {
            return Result.Failure<SongFile>(DomainServicesErrors.PassedDirectoryIsNotExists(songFilePath.FullName));
        }

        return new SongFile(songFilePath.FullName, string.Equals(songFilePath.Extension, DomainConstants.CueExtension, StringComparison.OrdinalIgnoreCase));
    }
}
