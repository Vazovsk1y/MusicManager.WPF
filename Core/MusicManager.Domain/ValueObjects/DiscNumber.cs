using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class DiscNumber : ValueObject<DiscNumber>
{
    public const string CD_KEYWORD = "CD";

    public int Digit { get; private set; }

    public string Value => $"{CD_KEYWORD}{Digit}";

    private DiscNumber() {  }

    public static Result<DiscNumber> Create(int number)
    {
        if (number <= 0)
        {
            return Result.Failure<DiscNumber>(DomainErrors.DiscNumber.DiscNumberDigitMustBeGreaterThanZero);
        }

        return new DiscNumber()
        {
            Digit = number,
        };
    }

    public static IEnumerable<DiscNumber> EnumerateRange(byte count = 5)
    {
        return Enumerable.Range(1, count).Select(e => new DiscNumber { Digit = e });
    }

	protected override IEnumerable<object?> GetEqualityComponents()
	{
		yield return Value;
	}
}

