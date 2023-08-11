using MusicManager.Domain.Common;
using MusicManager.Domain.Constants;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Entities;

public class PlaybackInfo : ValueObject<PlaybackInfo>
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongId SongId { get; private set; }

    public string ExecutableFileFullPath { get; }

    public CueInfo? CueInfo { get; private set; }

    public SongFileType ExecutableType { get; init; }

    public TimeSpan SongDuration { get; init; }

    #endregion

    #region --Constructors--

    protected PlaybackInfo() { } // for EF core

    private PlaybackInfo(string fullPath, SongId songId)
    {
        ExecutableFileFullPath = fullPath;
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
        string cueFileFullPath,
        TimeSpan index00,
        TimeSpan index01)
    {
        var creationResult = Create(fullPath, songId, duration);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var cueInfoCreationResult = CueInfo.Create(cueFileFullPath, index00, index01);

        if (cueInfoCreationResult.IsFailure)
        {
            return Result.Failure<PlaybackInfo>(cueInfoCreationResult.Error);
        }

        var songPlayInfo = creationResult.Value;
        songPlayInfo.CueInfo = cueInfoCreationResult.Value;
        return songPlayInfo;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExecutableFileFullPath;
        yield return ExecutableType;
        yield return SongDuration;
        yield return CueInfo;
    }

    #endregion
}
