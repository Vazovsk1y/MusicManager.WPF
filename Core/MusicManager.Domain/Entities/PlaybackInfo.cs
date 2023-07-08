using MusicManager.Domain.Constants;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Entities;

public class PlaybackInfo
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

    protected PlaybackInfo() { } // for EF core

    private PlaybackInfo(string fullPath, SongId songId)
    {
        ExecutableFileFullPath = fullPath;
        ExecutableFileName = Path.GetFileName(ExecutableFileFullPath);
        SongId = songId;
    }

    #endregion

    #region --Methods--

    internal static Result<PlaybackInfo> Create(
        string fullPath,
        SongId songId,
        TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return Result.Failure<PlaybackInfo>(DomainErrors.NullOrEmptyStringPassed());
        }

        string fileExtension = Path.GetExtension(fullPath);
        return fileExtension switch
        {
            DomainConstants.FlacExtension => new PlaybackInfo(fullPath, songId)
            {
                SongDuration = duration,
                ExecutableType = SongFileType.Flac,
            },
            DomainConstants.Mp3Extension => new PlaybackInfo(fullPath, songId)
            {
                SongDuration = duration,
                ExecutableType = SongFileType.Mp3,
            },
            DomainConstants.WVExtension => new PlaybackInfo(fullPath, songId)
            {
                SongDuration = duration,
                ExecutableType = SongFileType.WV,
            },
            DomainConstants.ApeExtension => new PlaybackInfo(fullPath, songId)
            {
                SongDuration = duration,
                ExecutableType = SongFileType.Ape,
            },
            _ => Result.Failure<PlaybackInfo>(DomainErrors.SongPlayInfo.UndefinedExecutableTypePassed(fileExtension))
        };
    }

    internal static Result<PlaybackInfo> Create(
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
            return Result.Failure<PlaybackInfo>(DomainErrors.NullOrEmptyStringPassed(nameof(cueFileFullPath)));
        }

        if (!cueFileFullPath.EndsWith(DomainConstants.CueExtension))
        {
            return Result.Failure<PlaybackInfo>(DomainErrors.SongPlayInfo.IncorrectCuePathPassed(cueFileFullPath));
        }

        var songPlayInfo = creationResult.Value;
        songPlayInfo.CueFilePath = cueFileFullPath;
        return songPlayInfo;
    }

    #endregion
}


