namespace MusicManager.Utils;

public static class YearsHelper 
{
    private static readonly List<int> _years = new();

    public static IEnumerable<int> Years
    {
        get
        {
            foreach (var item in _years)
            {
                yield return item;
            }
        }
    }

    static YearsHelper()
    {
        Initialize();
    }

    private static void Initialize()
    {
        var numbers = Enumerable.Range(1, DateTime.Now.Year).OrderDescending();
        _years.AddRange(numbers);
    }
}


