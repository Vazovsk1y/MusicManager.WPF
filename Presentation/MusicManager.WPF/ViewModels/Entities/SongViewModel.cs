using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Common;
using MusicManager.Domain.Models;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class SongViewModel : 
	ObservableObject, 
	IModifiable<SongViewModel>
{
	[ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private string? _discNumber;

	[ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private string _title = string.Empty;

	[ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private string? _type;

	[ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdatable))]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private int _number;

    public required SongId SongId { get; init; }

    public required DiscId DiscId { get; init; }

    public bool IsModified
    {
        get
        {
            return PreviousState?.Title != Title
                || PreviousState?.Type != Type
                || PreviousState?.Number != Number
                || PreviousState?.DiscNumber != DiscNumber;
        }
    }

    public string? IsUpdatable => IsModified ? "*" : null;

    public SongViewModel? PreviousState { get; private set; }

    public void RollBackChanges()
    {
        DiscNumber = PreviousState?.DiscNumber;
        Title = PreviousState?.Title;
        Type = PreviousState?.Type;
        Number = (int)PreviousState?.Number;
    }

    public void SetCurrentAsPrevious()
    {
        PreviousState = MemberwiseClone() as SongViewModel;
        OnPropertyChanged(nameof(IsModified));
        OnPropertyChanged(nameof(IsUpdatable));
    }
}
