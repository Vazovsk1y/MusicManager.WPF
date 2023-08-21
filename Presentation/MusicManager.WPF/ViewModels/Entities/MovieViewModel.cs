using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Models;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class MovieViewModel : 
    ObservableObject, 
    IModifiable<MovieViewModel>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private string _title = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private string? _productionCountry;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private string? _directorName;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private string? _directorLastName;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private int? _productionYear;

    public MovieViewModel? PreviousState { get; private set; }

    public bool IsModified
    {
        get
        {
            return PreviousState?.Title != Title
                || PreviousState?.ProductionCountry != ProductionCountry
                || PreviousState?.ProductionYear != ProductionYear
                || PreviousState?.DirectorLastName != DirectorLastName
                || PreviousState?.DirectorName != DirectorName;
        }
    }

    public string? IsUpdatable => IsModified ? "*" : null;

    private ObservableCollection<MovieReleaseViewModel>? _moviesReleasesViewsModels;

    public required MovieId MovieId { get; init; }

    public required SongwriterId SongwriterId { get; init; }

    public ObservableCollection<MovieReleaseViewModel> MoviesReleases
    {
        get => _moviesReleasesViewsModels ??= new();
        init => SetProperty(ref _moviesReleasesViewsModels, value);
    }

    public void SetCurrentAsPrevious()
    {
        PreviousState = new()
        {
            MovieId = MovieId,
            SongwriterId = SongwriterId,
            DirectorLastName = DirectorLastName,
            DirectorName = DirectorName,
            ProductionCountry = ProductionCountry,
            ProductionYear = ProductionYear,
            Title = Title,
        };
    }

    public void StartTrackingState()
    {
        OnPropertyChanged(nameof(IsModified));
        OnPropertyChanged(nameof(IsUpdatable));
    }

    public void RollBackChanges()
    {
        DirectorName = PreviousState?.DirectorName;
        DirectorLastName = PreviousState?.DirectorLastName;
        ProductionYear= PreviousState?.ProductionYear;
        ProductionCountry = PreviousState?.ProductionCountry;
        Title = PreviousState?.Title!;
    }
}

