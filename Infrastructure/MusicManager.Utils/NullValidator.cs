namespace MusicManager.Utils;

public static class NullValidator
{
    public static bool IsNotNull(params object?[] values)
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
