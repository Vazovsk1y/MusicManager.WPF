using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Models;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class MovieViewModel : 
    ObservableObject, 
    IModifiable<MovieViewModel>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private string _title = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private string? _productionCountry;

    [ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsModified))]
	[NotifyPropertyChangedFor(nameof(UpdatableSign))]
	private DirectorViewModel? _director;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private int _productionYear;

    public MovieViewModel PreviousState { get; private set; } = null!;

    public bool IsModified
    {
        get
        {
            return PreviousState.Title != Title
                || PreviousState.ProductionCountry != ProductionCountry
                || PreviousState.ProductionYear != ProductionYear
                || PreviousState.Director?.Id != Director?.Id;
        }
    }

    public string? UpdatableSign => IsModified ? "*" : null;

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
        PreviousState = (MovieViewModel)MemberwiseClone();
        OnPropertyChanged(nameof(IsModified));
        OnPropertyChanged(nameof(UpdatableSign));
    }

    public void RollBackChanges()
    {
        ProductionYear= PreviousState.ProductionYear;
        ProductionCountry = PreviousState.ProductionCountry;
        Title = PreviousState.Title;
        Director = PreviousState.Director;
    }
}
