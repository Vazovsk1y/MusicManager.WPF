using MusicManager.Domain.Common;
using MusicManager.Domain.Constants;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Helpers;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class SongFileInfo : ValueObject<SongFileInfo>
{
    public string Name { get; }

    public string FullPath { get; }

    public string? CueFilePath { get; private set; }

    public SongFileType Type { get; init; }

    public TimeSpan SongDuration { get; init; }

    private SongFileInfo(string fullPath) 
    {
        FullPath = fullPath;
        Name = Path.GetFileName(FullPath);
    }

    internal static Result<SongFileInfo> Create(
        string fullPath, 
        TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return Result.Failure<SongFileInfo>(DomainErrors.NullOrEmptyStringPassedError());
        }

        if (!PathValidator.IsValid(fullPath))
        {
            return Result.Failure<SongFileInfo>(new Error($"The passed path {fullPath} wasn't a file path."));
        }

        return Path.GetExtension(fullPath) switch
        {
            DomainConstants.FlacExtension => new SongFileInfo(fullPath)
            {
                SongDuration = duration,
                Type = SongFileType.Flac,
            },
            DomainConstants.Mp3Extension => new SongFileInfo(fullPath)
            {
                SongDuration = duration,
                Type = SongFileType.Mp3,
            },
            DomainConstants.WVExtension => new SongFileInfo(fullPath)
            {
                SongDuration = duration,
                Type = SongFileType.WV,
            },
            _ => Result.Failure<SongFileInfo>(new Error("Fjfkjf")),
        };
    }

    internal static Result<SongFileInfo> Create(
        string fullPath,
        TimeSpan duration,
        string cueFileFullPath)
    {
        var creationResult = Create(fullPath, duration);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        if (string.IsNullOrWhiteSpace(cueFileFullPath))
        {
            return Result.Failure<SongFileInfo>(DomainErrors.NullOrEmptyStringPassedError(nameof(cueFileFullPath)));
        }

        if (!PathValidator.IsValid(cueFileFullPath) || !cueFileFullPath.EndsWith(DomainConstants.CueExtension))
        {
            return Result.Failure<SongFileInfo>(new Error($"Incorrect cue path was passed {cueFileFullPath}."));
        }

        var songFileInfo = creationResult.Value;
        songFileInfo.CueFilePath = cueFileFullPath;
        return songFileInfo;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return FullPath;
        yield return Type;
        yield return SongDuration;
    }
}


