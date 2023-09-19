using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Entities;

namespace MusicManager.WPF.ViewModels.Entities;

public partial class DirectorViewModel : 
    ObservableObject,
    IModifiable<DirectorViewModel>
{
    public required DirectorId Id { get; init; }

    public bool IsModified => PreviousState.FullName != FullName;

    public DirectorViewModel PreviousState { get; private set; } = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModified))]
    private string _fullName = null!;

    public void RollBackChanges()
    {
        FullName = PreviousState.FullName;
    }

    public void SetCurrentAsPrevious()
    {
        PreviousState = (DirectorViewModel)MemberwiseClone();
        OnPropertyChanged(nameof(IsModified));
    }
}
