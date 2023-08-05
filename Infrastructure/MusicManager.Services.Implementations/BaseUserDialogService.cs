using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MusicManager.Services.Implementations
{
    public class BaseUserDialogService<T> : IUserDialogService<T> where T : Window
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected T? _window;

        public BaseUserDialogService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void CloseDialog()
        {
            _window?.Close();
            _window = null;
        }

        public void ShowDialog()
        {
            CloseDialog();

            var scope = _serviceScopeFactory.CreateScope();
            _window = scope.ServiceProvider.GetRequiredService<T>();

            _window.Closed += (_, _) =>
            {
                scope.Dispose();
                _window = null;
            };

            _window.ShowDialog();
        }
    }
}
