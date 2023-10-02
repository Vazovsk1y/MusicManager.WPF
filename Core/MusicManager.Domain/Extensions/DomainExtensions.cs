using MusicManager.Domain.Enums;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Extensions;

public static class DomainExtensions
{
    private const string FLAC_ROW = "Flac";
    private const string APE_ROW = "Ape";
    private const string WV_ROW = "WV";
    private const string MP3_ROW = "MP3";

    public static Result<DiscType> Create(this int number)
    {
        if (number <= 1)
        {
            return Result.Failure<DiscType>(new Error("Disc type starting with number must be positive number greater than 1."));
        }

        return new DiscType(number);
    }

    public static Result<SongFileType> CreateSongFileType(this string executableTypeRow) => executableTypeRow switch
    {
        FLAC_ROW => SongFileType.Flac,
        APE_ROW => SongFileType.Ape,
        WV_ROW => SongFileType.WV,
        MP3_ROW => SongFileType.Mp3,
        _ => Result.Failure<SongFileType>(new Error($"Unable to create song file type from {executableTypeRow}."))
    };

    public static string MapToString(this SongFileType songFileType) => songFileType switch
    {
        SongFileType.Mp3 => MP3_ROW,
        SongFileType.Flac => FLAC_ROW,
        SongFileType.WV => WV_ROW,
        SongFileType.Ape => APE_ROW,
        _ => SongFileType.Unknown.ToString(),
    };

    public static bool IsStoresIn(this IRoot root, string fullPath) => fullPath.StartsWith($"{root.RootPath}");

    public static string CombineWith(this IRoot root, string pathToCombineWith) => Path.Combine(root.RootPath, pathToCombineWith);
}
