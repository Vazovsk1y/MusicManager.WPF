using System.Globalization;

namespace MusicManager.Utils;

/// <summary>
/// Register as singleton.
/// </summary>
public interface ICountriesHelper
{
    IEnumerable<string> Countries { get; }
}

public class CountryHelper : ICountriesHelper
{
    private readonly List<string> _countriesNames = new();

    public IEnumerable<string> Countries
    {
        get
        {
            foreach (var item in _countriesNames.Order())
            {
                yield return item;
            }
        }
    }

    public CountryHelper()
    {
        Initialize();
    }

    private void Initialize()
    {
        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

        foreach (var cultureInfo in cultures)
        {
            try
            {
                RegionInfo region = new(cultureInfo.Name);

                if (!(_countriesNames.Contains(region.EnglishName)))
                {
                    _countriesNames.Add(region.EnglishName);
                }
            }
            catch (ArgumentException)
            {
                continue;
            }
        }
    }
}
