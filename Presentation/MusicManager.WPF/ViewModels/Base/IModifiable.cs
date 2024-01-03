namespace MusicManager.WPF.ViewModels.Entities;

public interface IModifiable<T> where T : IModifiable<T>
{
    bool IsModified { get; }

    T PreviousState { get; }

    void RollBackChanges();

    void SaveState();
}
