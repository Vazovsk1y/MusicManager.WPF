using MusicManager.Domain.Enums;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Extensions;

public static class DomainExtensions
{
    private const string CD_Row = "CD";
    private const string LP_Row = "LP";
    private const string BOOTLEG_Row = "Bootleg";
    private const string FLAC_ROW = "Flac";
    private const string APE_ROW = "Ape";
    private const string WV_ROW = "WV";
    private const string MP3_ROW = "MP3";
    private const string UNKNOWN_ROW = "Unknown";

    public static Result<DiscType> CreateDiscType(this string discTypeRow) => discTypeRow switch
    {
        CD_Row => DiscType.CD,
        LP_Row => DiscType.LP,
        BOOTLEG_Row => DiscType.Bootleg,
        "2CD" => DiscType.TwoCD,
        "3CD" => DiscType.ThreeeCD,
        UNKNOWN_ROW => DiscType.Unknown,
        _ => Result.Failure<DiscType>(new Error($"Unable to create disk type from {discTypeRow}."))
    };

    public static Result<SongFileType> CreateSongFileType(this string executableTypeRow) => executableTypeRow switch
    {
        FLAC_ROW => SongFileType.Flac,
        APE_ROW => SongFileType.Ape,
        WV_ROW => SongFileType.WV,
        MP3_ROW => SongFileType.Mp3,
        _ => Result.Failure<SongFileType>(new Error($"Unable to create song file type from {executableTypeRow}."))
    };

    public static string MapToString(this DiscType discType) => discType switch
    {
        DiscType.CD => CD_Row,
        DiscType.LP => LP_Row,
        DiscType.Bootleg => BOOTLEG_Row,
        DiscType.TwoCD => "2CD",
        DiscType.ThreeeCD => "3CD",
        DiscType.Unknown => "Unknown",
        _ => throw new KeyNotFoundException()
    };

    public static string MapToString(this SongFileType songFileType) => songFileType switch
    {
        SongFileType.Mp3 => MP3_ROW,
        SongFileType.Flac => FLAC_ROW,
        SongFileType.WV => WV_ROW,
        SongFileType.Ape => APE_ROW,
        _ => throw new KeyNotFoundException()
    };
}
