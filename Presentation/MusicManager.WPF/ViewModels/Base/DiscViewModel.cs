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
    private ObservableCollection<SongViewModel>? _songs;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    private int? _productionYear;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    private string? _productionCountry;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    private DiscType _selectedDiscType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    private string _identifier = string.Empty;

    public required DiscId DiscId { get; init; }

    public ObservableCollection<SongViewModel> Songs
    {
        get => _songs ??= new();
        init => SetProperty(ref _songs, value);
    }

    public T? PreviousState { get; private set; }

    public virtual bool IsModified
    {
        get
        {
            return PreviousState?.Identifier != Identifier
                || PreviousState?.SelectedDiscType != SelectedDiscType
                || PreviousState?.ProductionCountry != ProductionCountry
                || PreviousState?.ProductionYear != ProductionYear;
        }
    }

    public string? IsUpdatable => IsModified ? "*" : null;

    public virtual void RollBackChanges()
    {
        Identifier = PreviousState?.Identifier!;
        ProductionCountry = PreviousState?.ProductionCountry;
        ProductionYear = PreviousState?.ProductionYear;
        SelectedDiscType = PreviousState?.SelectedDiscType!;
    }

    public virtual void SetCurrentAsPrevious()
    {
        PreviousState = MemberwiseClone() as T;
        OnPropertyChanged(nameof(IsModified));
        OnPropertyChanged(nameof(IsUpdatable));
    }
}