using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class ProductionInfo : ValueObject<ProductionInfo>
{
    #region --Fields--

    public const string UndefinedCountry = "Undefined";

    public static readonly ProductionInfo None = new(null, null);

    #endregion

    #region --Properties--

    public string? Country { get; private set; }

    public int? Year { get; private set; }

    #endregion

    #region --Constructors--

    private ProductionInfo(string? country, int? year)
    {
        Country = country;
        Year = year;
    }

    #endregion

    #region --Methods--

    internal static Result<ProductionInfo> Create(string? country, int? year)
    {
        if (year is  null && string.IsNullOrWhiteSpace(country))
        {
            return new ProductionInfo(None.Country, None.Year);
        }

        if (year is not null)
        {
            if (IsYearCorrect((int)year))
            {
                return new ProductionInfo(country, year);
            }
            return Result.Failure<ProductionInfo>(DomainErrors.ProductInfo.IncorrectYearPassed(year.ToString()!));
        }

        return new ProductionInfo(country, year);
    }

    private static bool IsYearCorrect(int year) => year > 0 && year <= DateTime.Now.Year;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return Year;
    }

    #endregion
}