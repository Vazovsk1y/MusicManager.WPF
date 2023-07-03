using MusicManager.Domain.Constants;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Helpers;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Entities;

public class SongPlayInfo
{
    public SongId SongId { get; private set; }

    public string ExecutableFileName { get; }

    public string ExecutableFileFullPath { get; }

    public string? CueFilePath { get; private set; }

    public SongFileType ExecutableType { get; init; }

    public TimeSpan SongDuration { get; init; }

    protected SongPlayInfo() { } // for EF core

    private SongPlayInfo(string fullPath, SongId songId)
    {
        ExecutableFileFullPath = fullPath;
        ExecutableFileName = Path.GetFileName(ExecutableFileFullPath);
        SongId = songId;
    }

    internal static Result<SongPlayInfo> Create(
        string fullPath,
        SongId songId,
        TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return Result.Failure<SongPlayInfo>(DomainErrors.NullOrEmptyStringPassedError());
        }

        if (!PathValidator.IsValid(fullPath))
        {
            return Result.Failure<SongPlayInfo>(new Error($"The passed path {fullPath} wasn't a file path."));
        }

        return Path.GetExtension(fullPath) switch
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
            _ => Result.Failure<SongPlayInfo>(new Error("Song file extension is not supported.")),
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

        if (!PathValidator.IsValid(cueFileFullPath) || !cueFileFullPath.EndsWith(DomainConstants.CueExtension))
        {
            return Result.Failure<SongPlayInfo>(new Error($"Incorrect cue path was passed {cueFileFullPath}."));
        }

        var songPlayInfo = creationResult.Value;
        songPlayInfo.CueFilePath = cueFileFullPath;
        return songPlayInfo;
    }
}


