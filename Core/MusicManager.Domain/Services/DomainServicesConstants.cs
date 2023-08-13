using MusicManager.Domain.Constants;
using MusicManager.Domain.Enums;

namespace MusicManager.Domain.Services;

public static class DomainServicesConstants
{
    public const char SongwriterDirectoryNameSeparator = '.';

    public const char MovieDirectoryNameSeparator = '-';

    public const char DiscDirectoryNameSeparator = '-';

    public static string CD_KEYWORD => DiscType.CD.ToString();

    public const string MOVIES_FOLDER_NAME = "movies";

    public const string COMPILATIONS_FOLDER_NAME = "compilations";

    public const string COVERS_FOLDER_NAME = "covers";

    public const string FolderJPG = "folder.jpg";

    public static string[] AudioFilesExtensions => new[]
    {
        DomainConstants.WVExtension,
        DomainConstants.Mp3Extension,
        DomainConstants.ApeExtension,
        DomainConstants.FlacExtension,
    };
}