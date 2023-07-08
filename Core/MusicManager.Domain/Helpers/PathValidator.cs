using System.Text.RegularExpressions;

namespace MusicManager.Domain.Helpers;

public static partial class PathValidator
{
    [GeneratedRegex(@"^[a-zA-Z]:\\(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]*$")]
    private static partial Regex WindowsPathsNamingRules();

    [GeneratedRegex(@"^\.{1,}$")]
    private static partial Regex OnlyDotsInPathComponent();

    /// <summary>
    /// Checks if the passed path is valid. Available only full paths, not relative.
    /// </summary>
    /// <param name="path">The full path of file or directory.</param>
    /// <returns>true - if valid, otherwise false.</returns>
    public static bool IsValid(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        if (Path.EndsInDirectorySeparator(path))
        {
            return false;
        }    

        bool isPathNamingRulesMatches = WindowsPathsNamingRules().IsMatch(path);
        if (isPathNamingRulesMatches)
        {
            var pathComponents = path.Split(
                Path.DirectorySeparatorChar, 
                Path.AltDirectorySeparatorChar, 
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var component in pathComponents)
            {
                if (OnlyDotsInPathComponent().IsMatch(component))
                {
                    return false;
                }
            }
        }

        return isPathNamingRulesMatches;
    }
}
