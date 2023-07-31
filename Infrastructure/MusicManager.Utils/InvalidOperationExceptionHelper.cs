namespace MusicManager.Utils;

public static class InvalidOperationExceptionHelper
{
    public static void ThrowIfTrue(bool condition, string? exceptionText = null)
    {
        if (condition)
        {
            throw new InvalidOperationException(exceptionText ?? string.Empty);
        }
    }
}


