namespace MusicManager.Utils;

public static class DirectoryHelper
{
    public static string LocalApplicationDataPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public static DirectoryInfo CreateIfNotExists(string path)
    {
        ArgumentNullException.ThrowIfNull(nameof(path));

        var directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists) 
        {
            directoryInfo.Create();
        }

        return directoryInfo;
    }
}


