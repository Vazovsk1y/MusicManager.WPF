using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class ProductionInfo : ValueObject<ProductionInfo>
{
    #region --Fields--

    public const string UndefinedCountry = "Undefined";

    #endregion

    #region --Properties--

    public string Country { get; private set; } = string.Empty;

    public int? Year { get; private set; }

    #endregion

    #region --Constructors--

    private ProductionInfo(string country, int? year)
    {
        Country = country;
        Year = year;
    }

    #endregion

    #region --Methods--

    internal static Result<ProductionInfo> Create(string country, int? year)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return Result.Failure<ProductionInfo>(DomainErrors.NullOrEmptyStringPassed(nameof(country)));
        }

        if (year is null)
        {
            return new ProductionInfo(country, year);
        }

        if (IsYearCorrect((int)year))
        {
            return new ProductionInfo(country, year);
        }

        return Result.Failure<ProductionInfo>(DomainErrors.ProductInfo.IncorrectYearPassed(year.ToString()));
    }

    private static bool IsYearCorrect(int year) => year > 0 && year <= DateTime.Now.Year;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return Year;
    }

    #endregion
}