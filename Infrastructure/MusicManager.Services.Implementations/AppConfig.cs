using MusicManager.Services.Contracts;
using MusicManager.Utils;
using System.IO;
using System.Text.Json;

namespace MusicManager.Services.Implementations;

public class AppConfig : IAppConfig
{
    private readonly object _locker = new();

    private static readonly string _storageDirectoryPath = Path.Combine(DirectoryHelper.LocalApplicationDataPath, "Configs");

    public static readonly AppConfig Default = new()
    {
        RootPath = Environment.CurrentDirectory,
        DeleteAssociatedFolder = false,
        CreateAssociatedFolder = true,
    };

    public const string FileName = "appConfig.json";

    public static string FullPath => Path.Combine(_storageDirectoryPath, FileName);

    public string RootPath { get; set; } = null!;

    public bool DeleteAssociatedFolder { get; set; }

    public bool CreateAssociatedFolder { get; set; }

    public void Save()
    {
        if (!Directory.Exists(_storageDirectoryPath))
        {
            Directory.CreateDirectory(_storageDirectoryPath);
        }

        lock (_locker)
        {
            using var writer = new StreamWriter(FullPath);
            string json = JsonSerializer.Serialize(this);
            writer.Write(json);
        }
    }
}
