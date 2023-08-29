using MusicManager.Domain.Services;
using MusicManager.Services.Contracts.Base;
using System.IO;
using System.Text.Json;

namespace MusicManager.WPF;

internal class UserConfig :
    ISaveable,
    IRoot
{
    public const string FileName = "appConfig.json";

    public static readonly string SettingsFileFullPath = Path.Combine(App.AssociatedAppFolderFullPath, FileName);

    private readonly object _locker = new();

    public required string RootPath { get; init; }

    public required bool CreateAssociatedFolder { get; init; }

    public static readonly UserConfig Default = new()
    {
        RootPath = App.WorkingDirectory,
        CreateAssociatedFolder = true,
    };

    public void Save()
    {
        lock (_locker)
        {
            using var writer = new StreamWriter(SettingsFileFullPath);
            string json = JsonSerializer.Serialize(this);
            writer.Write(json);
        }
    }
}
