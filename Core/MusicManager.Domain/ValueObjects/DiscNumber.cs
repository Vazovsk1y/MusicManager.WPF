using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class DiscNumber : ValueObject<DiscNumber>
{
    public const string CD_KEYWORD = "CD";

    public int Digit { get; private set; }

    public string Value => $"{CD_KEYWORD}{Digit}";

    private DiscNumber() {  }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<DiscNumber> Create(int number)
    {
        if (int.IsNegative(number))
        {
            return Result.Failure<DiscNumber>(new Error("Disc number must greater than 0."));
        }

        return new DiscNumber()
        {
            Digit = number,
        };
    }

    public static IEnumerable<DiscNumber> EnumerateRange(byte count = 5)
    {
        return Enumerable.Range(1, count - 1).Select(e => new DiscNumber { Digit = e });
    }
}

