namespace MusicManager.Utils;

/// <summary>
/// Register as singleton.
/// </summary>
public interface IYearsHelper
{
    IEnumerable<string> Years { get; }
}

public class YearsHelper : IYearsHelper
{
    private readonly List<string> _years = new();

    public IEnumerable<string> Years
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
        var numbers = Enumerable.Range(1, DateTime.Now.Year).OrderDescending().Select(e => e.ToString());
        _years.AddRange(numbers);
    }
}


