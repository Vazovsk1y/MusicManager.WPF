using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.Domain.Entities;
using System;

namespace MusicManager.WPF.ViewModels.Entities;

public partial class DirectorViewModel : 
    ObservableObject,
    IEquatable<DirectorViewModel>,
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

    public void SaveState()
    {
        PreviousState = (DirectorViewModel)MemberwiseClone();
        OnPropertyChanged(nameof(IsModified));
    }

    public bool Equals(DirectorViewModel? other)
    {
        if (other is null)
        {
            return false;
        }

        return other.Id == Id;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return HashCode.Combine(Id, FullName) * 44;
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DirectorViewModel);
    }
}
