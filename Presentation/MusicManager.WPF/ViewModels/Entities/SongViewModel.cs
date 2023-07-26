using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Common;
using MusicManager.Domain.Models;

namespace MusicManager.WPF.ViewModels.Entities;

internal class SongViewModel : ObservableObject
{
	private string? _discNumber;

    private string _title = string.Empty;

	private string? _type;

    public required SongId SongId { get; init; }

    public required DiscId DiscId { get; init; }

    public string? DiscNumber
	{
		get => _discNumber;
		set => SetProperty(ref _discNumber, value);
	}

	public string Title
	{
		get => _title;
		set => SetProperty(ref _title, value);
	}
			
	public string? ExecutableType
	{
		get => _type;
		set => SetProperty(ref _type, value);
	}
}
