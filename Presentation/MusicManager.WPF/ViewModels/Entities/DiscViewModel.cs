using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Common;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal abstract class DiscViewModel : ObservableObject
{
	private int _productionYear;

	private ObservableCollection<SongViewModel>? _songs;

    private string _productionCountry = string.Empty;

    private string _type = string.Empty;

	public required DiscId DiscId { get; init; }

    public int ProductionYear
	{
		get => _productionYear;
		set => SetProperty(ref _productionYear, value);
	}

	public string ProductionCountry
	{
		get => _productionCountry;
		set => SetProperty(ref _productionCountry, value);
	}

	public string Type
	{
		get => _type;
		set => SetProperty(ref _type, value);
	}

    public ObservableCollection<SongViewModel> Songs
    {
        get => _songs ??= new();
        init => SetProperty(ref _songs, value);
    }
}
