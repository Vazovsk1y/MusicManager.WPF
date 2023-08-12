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

    public static (bool isCreated, string message) TryToCreateIfNotExists(string path, out DirectoryInfo? directoryInfo)
    {
        try
        {
            directoryInfo = CreateIfNotExists(path);
            return (true, string.Empty);
        }
        catch(Exception ex)
        {
            directoryInfo = null;
            return (false, ex.Message);
        }
    }
}


