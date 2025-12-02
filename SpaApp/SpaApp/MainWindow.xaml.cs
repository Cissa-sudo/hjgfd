using System.Windows;
using System.Windows.Input;
using SpaApp.Models;

namespace SpaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Subscription subscription)
            {
                MessageBox.Show($"Выбран абонемент: {subscription.Name}\n" +
                              $"Цена: {subscription.Price} руб.\n" +
                              $"Тип: {subscription.TypeName}\n" +
                              $"Заведение: {subscription.FacilityName}",
                    "Выбор абонемента",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = false;
        }
    }
}
