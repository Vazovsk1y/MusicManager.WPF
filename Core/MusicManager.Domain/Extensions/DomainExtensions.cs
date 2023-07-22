using MusicManager.Domain.Enums;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Extensions;

public static class DomainExtensions
{
    private const string CD_Row = "CD";
    private const string LP_Row = "LP";
    private const string BOOTLEG_Row = "Bootleg";

    public static Result<DiscType> CreateDiscType(this string discTypeRow) => discTypeRow switch
    {
        CD_Row => DiscType.CD,
        LP_Row => DiscType.LP,
        BOOTLEG_Row => DiscType.Bootleg,
        "2CD" => DiscType.TwoCD,
        "3CD" => DiscType.ThreeeCD,
        _ => Result.Failure<DiscType>(new Error($"Unable to create disk type from {discTypeRow}."))
    };

    public static Result<SongFileType> CreateSongFileType(this string executableTypeRow) => executableTypeRow switch
    {
        "Flac" => SongFileType.Flac,
        "Ape" => SongFileType.Ape,
        "WV" => SongFileType.WV,
        "MP3" => SongFileType.Mp3,
        _ => Result.Failure<SongFileType>(new Error($"Unable to create song file type from {executableTypeRow}."))
    };

    public static string MapToString(this DiscType discType) => discType switch
    {
        DiscType.CD => CD_Row,
        DiscType.LP => LP_Row,
        DiscType.Bootleg => BOOTLEG_Row,
        DiscType.TwoCD => "2CD",
        DiscType.ThreeeCD => "3CD",
        _ => throw new KeyNotFoundException()
    };
}
