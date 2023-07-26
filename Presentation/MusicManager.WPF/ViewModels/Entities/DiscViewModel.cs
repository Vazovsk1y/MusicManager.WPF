﻿using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Common;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal abstract class DiscViewModel : ObservableObject
{
	private string? _productionYear;

	private ObservableCollection<SongViewModel>? _songs;

	private string? _productionCountry;

    private string _type = string.Empty;

	public required DiscId DiscId { get; init; }

    public string? ProductionYear
	{
		get => _productionYear;
		set => SetProperty(ref _productionYear, value);
	}

	public string? ProductionCountry
	{
		get => _productionCountry;
		set => SetProperty(ref _productionCountry, value);
	}

	public string DiscType
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
