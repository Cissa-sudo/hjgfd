using System.Windows;
using SpaApp.ViewModels;
using SpaApp.Models;

namespace SpaApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем главное окно, но не показываем его сразу
            var mainWindow = new MainWindow();
            var viewModel = new MainViewModel();
            mainWindow.DataContext = viewModel;

            // Показываем окно входа
            var loginWindow = new LoginWindow();
            var loginResult = loginWindow.ShowDialog();

            if (loginResult == true && loginWindow.IsSuccess)
            {
                // Устанавливаем пользователя
                viewModel.CurrentUser = new User
                {
                    Username = loginWindow.Username,
                    FullName = loginWindow.FullName,
                    Email = loginWindow.Email
                };

                // Показываем главное окно
                mainWindow.Show();
            }
            else
            {
                // Закрываем приложение если вход отменен
                Shutdown();
            }
        }
    }
}