namespace MusicManager.Domain.Services.Implementations.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveAllSpaces(this string row) => row.Replace(" ", string.Empty);
    }
}
