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

    public string ExecutableFilePath { get; }

    public AudioType AudioType { get; init; }

    public TimeSpan Duration { get; init; }

    public CueInfo? CueInfo { get; private set; }

	#endregion

	#region --Constructors--

#pragma warning disable CS8618 
	protected PlaybackInfo() { } // for EF core

#pragma warning restore CS8618 

	private PlaybackInfo(string path, SongId songId)
    {
        ExecutableFilePath = path;
        SongId = songId;
    }

    #endregion

    #region --Methods--

    internal static Result<PlaybackInfo> Create(
        string executableFilePath,
        SongId songId,
        TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(executableFilePath))
        {
            return Result.Failure<PlaybackInfo>(DomainErrors.NullOrEmptyStringPassed("executable song file path"));
        }

        string fileExtension = Path.GetExtension(executableFilePath);
        return fileExtension.ToLower() switch
        {
            DomainConstants.FlacExtension => new PlaybackInfo(executableFilePath, songId)
            {
                Duration = duration,
                AudioType = AudioType.Flac,
            },
            DomainConstants.Mp3Extension => new PlaybackInfo(executableFilePath, songId)
            {
                Duration = duration,
                AudioType = AudioType.Mp3,
            },
            DomainConstants.WVExtension => new PlaybackInfo(executableFilePath, songId)
            {
                Duration = duration,
                AudioType = AudioType.WV,
            },
            DomainConstants.ApeExtension => new PlaybackInfo(executableFilePath, songId)
            {
                Duration = duration,
                AudioType = AudioType.Ape,
            },
            _ => new PlaybackInfo(executableFilePath, songId)
            {
                Duration = duration,
                AudioType = AudioType.Unknown,
            }
        };
    }

    internal static Result<PlaybackInfo> Create(
        string executableFilePath,
        SongId songId,
        TimeSpan duration,
        string cueFilePath,
        TimeSpan index00,
        TimeSpan index01,
        string songNameInCueFile)
    {
        if (Path.GetDirectoryName(executableFilePath) != Path.GetDirectoryName(cueFilePath)) 
        {
            return Result.Failure<PlaybackInfo>(DomainErrors.PlaybackInfo.CueFileNotPlacedInTheExecutableFileFolder);
        }

        var creationResult = Create(executableFilePath, songId, duration);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var cueInfoCreationResult = CueInfo.Create(cueFilePath, index00, index01, songNameInCueFile);
        if (cueInfoCreationResult.IsFailure)
        {
            return Result.Failure<PlaybackInfo>(cueInfoCreationResult.Error);
        }

        var cueInfo = cueInfoCreationResult.Value;
		var playbackInfo = creationResult.Value;
        playbackInfo.CueInfo = cueInfo;
        return playbackInfo;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExecutableFilePath;
        yield return AudioType;
        yield return Duration;
        yield return CueInfo;
    }

    #endregion
}
