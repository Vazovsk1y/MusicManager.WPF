using MusicManager.Domain.Enums;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Extensions;

public static class StringExtensions
{
    public static Result<DiscType> CreateDiscType(this string discType) => discType switch
    {
        "CD" => DiscType.CD,
        "2CD" => DiscType.TwoCD,
        "LP" => DiscType.LP,
        "3CD" => DiscType.ThreeeCD,
        "Bootleg" => DiscType.Bootleg,
        _ => Result.Failure<DiscType>(new Error($"Unable to create disk type from {discType}."))
    };

    public static string MapToString(this DiscType discType) => discType switch
    {
        DiscType.CD => "CD",
        DiscType.LP => "LP",
        DiscType.Bootleg => "Bootleg",
        DiscType.TwoCD => "2CD",
        DiscType.ThreeeCD => "3CD",
        _ => throw new KeyNotFoundException()
    };
}
