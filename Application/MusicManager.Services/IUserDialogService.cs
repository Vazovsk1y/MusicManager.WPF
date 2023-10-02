namespace MusicManager.Services;

public interface IUserDialogService<TDialogProvider>
{
    void ShowDialog();

    void CloseDialog();

    void ShowDialog<TDialogData>(TDialogData data);
}
