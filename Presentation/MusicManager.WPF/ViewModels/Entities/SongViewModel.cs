using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using System;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class SongViewModel : 
	ObservableObject, 
	IUpdatable<SongViewModel>
{
	[ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    private string? _discNumber;

	[ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private string _title = string.Empty;

	[ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private string? _type;

	[ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(UpdatableSign))]
    private int _number;

    [ObservableProperty]
    private TimeSpan _duration;

    public required SongId SongId { get; init; }

    public required DiscId DiscId { get; init; }

    public bool IsUpdatable
    {
        get
        {
            return PreviousState.Title != Title
                || PreviousState.Number != Number;
        }
    }

    public string? UpdatableSign => IsUpdatable ? "*" : null;

    public SongViewModel PreviousState { get; private set; } = null!;

    public void RollBackChanges()
    {
        DiscNumber = PreviousState.DiscNumber;
        Title = PreviousState.Title;
        Type = PreviousState.Type;
        Number = PreviousState.Number;
    }

    public void SetCurrentAsPrevious()
    {
        PreviousState = (SongViewModel)MemberwiseClone();
        OnPropertyChanged(nameof(IsUpdatable));
        OnPropertyChanged(nameof(UpdatableSign));
    }
}
