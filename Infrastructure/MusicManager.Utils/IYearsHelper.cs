namespace MusicManager.Utils;

/// <summary>
/// Register as singleton.
/// </summary>
public interface IYearsHelper
{
    IEnumerable<int> Years { get; }
}

public class YearsHelper : IYearsHelper
{
    private readonly List<int> _years = new();

    public IEnumerable<int> Years
    {
        get
        {
            foreach (var item in _years)
            {
                yield return item;
            }
        }
    }

    public YearsHelper()
    {
        Initialize();
    }

    private void Initialize()
    {
        var numbers = Enumerable.Range(1, DateTime.Now.Year).OrderDescending();
        _years.AddRange(numbers);
    }
}


