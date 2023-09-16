namespace MusicManager.WPF.ViewModels.Entities;

public interface IUpdatable<T> where T : IUpdatable<T>
{
    bool IsUpdatable { get; }

    T PreviousState { get; }

    void RollBackChanges();

    void SetCurrentAsPrevious();
}
