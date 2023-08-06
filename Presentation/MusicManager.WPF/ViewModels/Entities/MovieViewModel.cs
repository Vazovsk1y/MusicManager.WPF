using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Models;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal class MovieViewModel : ObservableObject
{
    private string _title = string.Empty;

    private string? _productionCountry;

    private string? _directorFullName;

    private int? _productionYear;

    private ObservableCollection<MovieReleaseViewModel>? _moviesReleasesViewsModels;

    public required MovieId MovieId { get; init; }

    public required SongwriterId SongwriterId { get; init; }    

    public ObservableCollection<MovieReleaseViewModel> MoviesReleases
    {
        get => _moviesReleasesViewsModels ??= new();
        init => SetProperty(ref _moviesReleasesViewsModels, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public int? ProductionYear
    {
        get => _productionYear;
        set => SetProperty(ref _productionYear, value);
    }

    public string? ProductionCountry
    {
        get => _productionCountry;
        set => SetProperty(ref _productionCountry, value);
    }

    public string? DirectorFullName
    {
        get => _directorFullName;
        set => SetProperty(ref _directorFullName, value);
    }
}
