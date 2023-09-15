using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.ValueObjects;

public partial class DiscType : ValueObject<DiscType>
{
    internal const string CD_Row = "CD";
    internal const string LP_Row = "LP";
    internal const string BOOTLEG_Row = "Bootleg";
    internal const string UNKNOWN_ROW = "Unknown";

    public static readonly DiscType Unknown = new(UNKNOWN_ROW);
    public static readonly DiscType CD = new(CD_Row);
    public static readonly DiscType LP = new(LP_Row);
    public static readonly DiscType Bootleg = new(BOOTLEG_Row);

    public string Value { get; private set; }

    private DiscType(string value) 
    {
        Value = value;
    }

    internal DiscType(int numberPrefix)
    {
        Value = $"{numberPrefix}CD";
    }

    public static Result<DiscType> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<DiscType>(DomainErrors.NullOrEmptyStringPassed());
        }

        return value switch
        {
            CD_Row => CD,
            LP_Row => LP,
            BOOTLEG_Row => Bootleg,
            UNKNOWN_ROW => Unknown,
            _ when IsDiscTypeStartingFromNumber().IsMatch(value) => new DiscType(value),
            _ => Unknown,
        };
    }

    [GeneratedRegex(@"^([2-9]|[1-9]\d+)\d*CD$")]
    private static partial Regex IsDiscTypeStartingFromNumber();

    public static IEnumerable<DiscType> EnumerateRange(byte count = 5)
    {
        return new List<DiscType>(Enumerable.Range(2, count - 1).Select(e => new DiscType(e)))
        {
            CD,
            LP,
            Bootleg,
            Unknown,
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}