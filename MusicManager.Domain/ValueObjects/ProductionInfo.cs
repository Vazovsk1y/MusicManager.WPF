using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class ProductionInfo
{
    #region --Fields--

    public static readonly ProductionInfo None = new("Undefined", "Undefined");

    #endregion

    #region --Properties--

    public string Country { get; private set; } = string.Empty;

    public string Year { get; private set; }

    #endregion

    #region --Constructors--

    private ProductionInfo(string country, string year)
    {
        Country = country;
        Year = year;
    }

    #endregion

    #region --Methods--

    public static Result<ProductionInfo> Create(string country, string year)
    {
        if (string.IsNullOrEmpty(country) || string.IsNullOrEmpty(year))
        {
            return Result.Failure<ProductionInfo>(DomainErrors.NullOrEmptyStringPassedError());
        }

        if (int.TryParse(year, out int result) && IsYearCorrect(result))
        {
            return new ProductionInfo(country, year);
        }

        return Result.Failure<ProductionInfo>(DomainErrors.ProductInfoErrors.IncorrectYearPassed);
    }

    public static Result<ProductionInfo> Create(string country, int year)
    {
        if (string.IsNullOrEmpty(country))
        {
            return Result.Failure<ProductionInfo>(DomainErrors.NullOrEmptyStringPassedError(nameof(country)));
        }

        if (IsYearCorrect(year))
        {
            return new ProductionInfo(country, year.ToString());
        }

        return Result.Failure<ProductionInfo>(DomainErrors.ProductInfoErrors.IncorrectYearPassed);
    }

    private static bool IsYearCorrect(int year) => year > 0 && year <= DateTime.Now.Year;

    #endregion
}