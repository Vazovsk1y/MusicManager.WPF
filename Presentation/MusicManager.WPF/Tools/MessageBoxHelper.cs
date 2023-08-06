using System.Windows;

namespace MusicManager.WPF.Tools;

public static class MessageBoxHelper
{
    public static void ShowErrorBox(string message, string caption = "Error")
    {
        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void ShowInfoBox(string message, string caption = "Info")
    {
        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

