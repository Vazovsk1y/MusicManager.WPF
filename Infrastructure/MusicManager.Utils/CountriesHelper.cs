using System.Globalization;

namespace MusicManager.Utils;

public static class CountryHelper 
{
    private static readonly List<string> _countriesNames = new();

    public static IEnumerable<string> Countries
    {
        get
        {
            return _countriesNames.AsReadOnly();
        }
    }

    static CountryHelper()
    {
        Initialize();
    }

    private static void Initialize()
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
