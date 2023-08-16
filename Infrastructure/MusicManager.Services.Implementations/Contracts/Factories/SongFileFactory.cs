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
        if (songFilePath is { Exists: false })
        {
            return Result.Failure<SongFile>(DomainServicesErrors.PassedFileIsNotExists(songFilePath.FullName));
        }

        return new SongFile(songFilePath.FullName, songFilePath.Extension == DomainConstants.CueExtension);
    }
}
