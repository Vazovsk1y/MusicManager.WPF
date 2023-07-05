using MusicManager.Domain.Constants;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Entities;

public class SongPlayInfo
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongId SongId { get; private set; }

    public string ExecutableFileName { get; }

    public string ExecutableFileFullPath { get; }

    public string? CueFilePath { get; private set; }

    public SongFileType ExecutableType { get; init; }

    public TimeSpan SongDuration { get; init; }

    #endregion

    #region --Constructors--

    protected SongPlayInfo() { } // for EF core

    private SongPlayInfo(string fullPath, SongId songId)
    {
        ExecutableFileFullPath = fullPath;
        ExecutableFileName = Path.GetFileName(ExecutableFileFullPath);
        SongId = songId;
    }

    #endregion

    #region --Methods--

    internal static Result<SongPlayInfo> Create(
        string fullPath,
        SongId songId,
        TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return Result.Failure<SongPlayInfo>(DomainErrors.NullOrEmptyStringPassedError());
        }

        string fileExtension = Path.GetExtension(fullPath);
        return fileExtension switch
        {
            DomainConstants.FlacExtension => new SongPlayInfo(fullPath, songId)
            {
                SongDuration = duration,
                ExecutableType = SongFileType.Flac,
            },
            DomainConstants.Mp3Extension => new SongPlayInfo(fullPath, songId)
            {
                SongDuration = duration,
                ExecutableType = SongFileType.Mp3,
            },
            DomainConstants.WVExtension => new SongPlayInfo(fullPath, songId)
            {
                SongDuration = duration,
                ExecutableType = SongFileType.WV,
            },
            DomainConstants.ApeExtension => new SongPlayInfo(fullPath, songId)
            {
                SongDuration = duration,
                ExecutableType = SongFileType.Ape,
            },
            _ => Result.Failure<SongPlayInfo>(DomainErrors.SongPlayInfo.UndefinedExecutableTypePassed(fileExtension))
        };
    }

    internal static Result<SongPlayInfo> Create(
        string fullPath,
        SongId songId,
        TimeSpan duration,
        string cueFileFullPath)
    {
        var creationResult = Create(fullPath, songId, duration);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        if (string.IsNullOrWhiteSpace(cueFileFullPath))
        {
            return Result.Failure<SongPlayInfo>(DomainErrors.NullOrEmptyStringPassedError(nameof(cueFileFullPath)));
        }

        if (!cueFileFullPath.EndsWith(DomainConstants.CueExtension))
        {
            return Result.Failure<SongPlayInfo>(DomainErrors.SongPlayInfo.IncorrectCuePathPassed(cueFileFullPath));
        }

        var songPlayInfo = creationResult.Value;
        songPlayInfo.CueFilePath = cueFileFullPath;
        return songPlayInfo;
    }

    #endregion
}


