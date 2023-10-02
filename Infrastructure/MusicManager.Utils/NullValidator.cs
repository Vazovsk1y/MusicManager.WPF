namespace MusicManager.Utils;

public static class NullValidator
{
    public static bool IsAnyNull(params object?[] values)
    {
        foreach (object? value in values)
        {
            if (value is null)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsAllNotNull(params object?[] values)
    {
        foreach (object? value in values)
        {
            if (value is null)
            {
                return false;
            }
        }

        return true;
    }
}
