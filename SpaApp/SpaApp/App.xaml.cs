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
            var mainWindow = new MainWindow();
            var viewModel = new MainViewModel();
            mainWindow.DataContext = viewModel;

            var loginWindow = new LoginWindow();
            var loginResult = loginWindow.ShowDialog();

            if (loginResult == true && loginWindow.IsSuccess)
            {
                viewModel.CurrentUser = new User
                {
                    Username = loginWindow.Username,
                    FullName = loginWindow.FullName,
                    Email = loginWindow.Email
                };

                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
