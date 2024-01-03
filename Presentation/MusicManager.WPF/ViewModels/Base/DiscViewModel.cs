using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class DiscViewModel<T> : 
    ObservableObject,
    IDiscViewModel,
    IModifiable<T> where T : DiscViewModel<T>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private int? _productionYear;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private string? _productionCountry;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private DiscType _selectedDiscType = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private string _identifier = null!;

    public required DiscId DiscId { get; init; }

    [ObservableProperty]
    private ObservableCollection<SongViewModel> _songs = new();

    public bool IsSongsLoaded { get; set; }

    public T PreviousState { get; private set; } = null!;

    public virtual bool IsModified
    {
        get
        {
            return PreviousState.Identifier != Identifier
                || PreviousState.SelectedDiscType != SelectedDiscType
                || PreviousState.ProductionCountry != ProductionCountry
                || PreviousState.ProductionYear != ProductionYear;
        }
    }

    public string? UpdatableSign => IsModified ? "*" : null;

    public virtual void RollBackChanges()
    {
        Identifier = PreviousState.Identifier;
        ProductionCountry = PreviousState.ProductionCountry;
        ProductionYear = PreviousState.ProductionYear;
        SelectedDiscType = PreviousState.SelectedDiscType;
    }

    public virtual void SaveState()
    {
        PreviousState = (T)MemberwiseClone();
        OnPropertyChanged(nameof(IsModified));
        OnPropertyChanged(nameof(UpdatableSign));
    }
}