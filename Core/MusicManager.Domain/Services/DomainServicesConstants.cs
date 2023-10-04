using MusicManager.Domain.Constants;

namespace MusicManager.Domain.Services;

public static class DomainServicesConstants
{
    public const char SongwriterFolderNameSeparator = '.';

    public const char MovieFolderNameSeparator = '-';

    public const char DiscFolderNameSeparator = '-';

    public const string MOVIES_FOLDER_NAME = "movies";

    public const string COMPILATIONS_FOLDER_NAME = "compilations";

    public const string COVERS_FOLDER_NAME = "covers";

    public const string FolderJPGFileName = "folder.jpg";

    public static readonly string[] AudioFilesExtensions = new[]
    {
        DomainConstants.WVExtension,
        DomainConstants.Mp3Extension,
        DomainConstants.ApeExtension,
        DomainConstants.FlacExtension,
    };
}