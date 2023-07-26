using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Models;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal class SongwriterViewModel : ObservableObject
{
	private ObservableCollection<CompilationViewModel>? _compilations;

	private ObservableCollection<MovieViewModel>? _movies;

	private string _name = string.Empty;

	private string _surname = string.Empty;

	public required SongwriterId SongwriterId { get; init; }

	public ObservableCollection<CompilationViewModel> Compilations 
	{ 
		get => _compilations ??= new(); 
		init => SetProperty(ref _compilations, value);
	}

    public ObservableCollection<MovieViewModel> Movies 
	{ 
		get => _movies ??= new();
		init => SetProperty(ref _movies, value); 
	}

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Surname
    {
        get => _surname;
        set => SetProperty(ref _surname, value);
    }

	public string FullName => $"{Name} {Surname}";
}
