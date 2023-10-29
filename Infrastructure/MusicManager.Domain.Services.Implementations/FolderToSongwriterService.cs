using Microsoft.Extensions.Logging;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Services.Storage;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations;

public class FolderToSongwriterService : 
    BaseDomainService,
    IFolderToSongwriterService
{
    #region --Fields--

    

    #endregion

    #region --Properties--



    #endregion

    #region --Constructors--

    public FolderToSongwriterService(IRoot userConfig, ILogger<FolderToSongwriterService> logger) : base(userConfig, logger) { }

    #endregion

    #region --Methods--

    public Task<Result<Songwriter>> GetEntityAsync(string songwriterPath)
    {
        var isAbleToMoveNextResult = IsAbleToParse<DirectoryInfo>(songwriterPath);
        if (isAbleToMoveNextResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Songwriter>(isAbleToMoveNextResult.Error));
        }

        var directoryInfo = isAbleToMoveNextResult.Value;
        if (directoryInfo.EnumerateFiles().FirstOrDefault(e => e.Name == SongwriterEntityJson.FileName) is FileInfo fileInfo)
        {
            var entityJsonResult = GetEntityInfoFromJsonFile<SongwriterEntityJson, Songwriter>(fileInfo);
            return entityJsonResult.IsFailure ?
                Task.FromResult(Result.Failure<Songwriter>(entityJsonResult.Error))
                :
                Task.FromResult(entityJsonResult.Value.ToEntity(songwriterPath.GetRelational(_root)));
        }

        var (isInfoSuccessfullyExtracted, name, surname) = GetSongwriterInfoFromDirectoryName(directoryInfo.Name);
        if (!isInfoSuccessfullyExtracted)
        {
            return Task.FromResult(Result.Failure<Songwriter>(DomainServicesErrors.PassedDirectoryNamedIncorrect(songwriterPath)));
        }

        var songwriterCreationResult = Songwriter.Create(name!, surname!, songwriterPath.GetRelational(_root));
        if (songwriterCreationResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Songwriter>(songwriterCreationResult.Error));
        }

        var songwriter = songwriterCreationResult.Value;
        return Task.FromResult(Result.Success(songwriter));
    }

    private (bool isInfoSuccessfullyExtracted, string? name, string? surname) GetSongwriterInfoFromDirectoryName(string directoryName)
    {
        var info = directoryName.Split(DomainServicesConstants.SongwriterFolderNameSeparator, StringSplitOptions.RemoveEmptyEntries);

        return info.Length < 2 ? (false, null, null) : (true, info[0], info[1]);
    }

    #endregion
}
