using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class DiscViewModel<T> : 
    ObservableObject,
    IDiscViewModel,
    IUpdatable<T> where T : DiscViewModel<T>
{
    private ObservableCollection<SongViewModel>? _songs;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private int? _productionYear;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private string? _productionCountry;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private DiscType _selectedDiscType = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private string _identifier = null!;

    public required DiscId DiscId { get; init; }

    public ObservableCollection<SongViewModel> Songs
    {
        get => _songs ??= new();
        init => SetProperty(ref _songs, value);
    }

    public T PreviousState { get; private set; } = null!;

    public virtual bool IsUpdatable
    {
        get
        {
            return PreviousState.Identifier != Identifier
                || PreviousState.SelectedDiscType != SelectedDiscType;
        }
    }

    public string? UpdatableSign => IsUpdatable ? "*" : null;

    public virtual void RollBackChanges()
    {
        Identifier = PreviousState.Identifier;
        ProductionCountry = PreviousState.ProductionCountry;
        ProductionYear = PreviousState.ProductionYear;
        SelectedDiscType = PreviousState.SelectedDiscType;
    }

    public virtual void SetCurrentAsPrevious()
    {
        PreviousState = (T)MemberwiseClone();
        OnPropertyChanged(nameof(IsUpdatable));
        OnPropertyChanged(nameof(UpdatableSign));
    }
}