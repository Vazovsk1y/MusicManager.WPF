using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations;

public class DirectoryToSongwriterService : IStorageToSongwriterService
{
    private const char Separator = '.';

    public Task<Result<Songwriter>> GetEntityAsync(IStorage storage)
    {
        var (isInfoSuccessfullyExtracted, name, surname) = GetSongwriterInfo(storage.Name);
        if (!isInfoSuccessfullyExtracted)
        {
            return Task.FromResult(Result.Failure<Songwriter>(DomainServicesErrors.PassedDirectoryNamedIncorrect(storage.FullPath)));
        }

        var songwriterCreationResult = Songwriter.Create(name!, surname!, storage.Name, storage.FullPath);
        if (songwriterCreationResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Songwriter>(songwriterCreationResult.Error));
        }

        var songWriter = songwriterCreationResult.Value;
        return Task.FromResult(Result.Success(songWriter));
    }

    private (bool isInfoSuccessfullyExtracted, string? name, string? surname) GetSongwriterInfo(string directoryName)
    {
        var info = directoryName.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

        return info.Length < 2 ? (false, null, null) : (true, info[0], info[1]);
    }
}
