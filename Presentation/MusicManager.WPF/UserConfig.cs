using MusicManager.Domain.Services;
using MusicManager.Services.Contracts.Base;
using System.IO;
using System.Text.Json;

namespace MusicManager.WPF;

internal interface IUserConfig : ISaveable, IRoot
{
	bool CreateAssociatedFolder { get; set; }
}

internal class UserConfig : IUserConfig
{
	public const string FileName = "appConfig.json";

	public static readonly string SettingsFileFullPath = System.IO.Path.Combine(App.AssociatedAppFolderFullPath, FileName);

	private readonly object _locker = new();

	public required string Path { get; set; }

	public required bool CreateAssociatedFolder { get; set; }

	public static readonly UserConfig Default = new()
	{
		Path = App.WorkingDirectory,
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
