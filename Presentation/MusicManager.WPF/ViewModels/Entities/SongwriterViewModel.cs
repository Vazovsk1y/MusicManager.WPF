using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Models;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal class SongwriterViewModel : ObservableObject
{
	private ObservableCollection<CompilationViewModel>? _compilations;

	private ObservableCollection<MovieViewModel>? _movies;

	private string? _fullName;

	public required SongwriterId SongwriterId { get; init; }

	public ObservableCollection<CompilationViewModel> Compilations 
	{ 
		get => _compilations ??= new(); 
		set => SetProperty(ref _compilations, value);
	}

    public ObservableCollection<MovieViewModel> Movies 
	{ 
		get => _movies ??= new();
		set => SetProperty(ref _movies, value); 
	}

	public string? FullName
	{
		get => _fullName;
		set => SetProperty(ref _fullName, value);
	}

	public bool IsCompilationsLoaded { get; set; }

	public bool IsMoviesLoaded { get; set; }
}
