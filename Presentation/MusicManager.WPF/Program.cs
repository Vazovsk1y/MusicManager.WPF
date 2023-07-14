using System;

namespace MusicManager.WPF;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
