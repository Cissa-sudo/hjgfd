using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using SpaApp.Models;
using SpaApp.ViewModels;

namespace SpaApp
{
    public partial class ComparisonWindow : Window
    {
        private MainViewModel _mainViewModel;

        public ComparisonWindow(List<Subscription> subscriptions, MainViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            DataContext = new ComparisonViewModel(subscriptions, _mainViewModel);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveSubscription_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Subscription subscription)
            {
                var viewModel = DataContext as ComparisonViewModel;
                viewModel?.RemoveFromComparison(subscription);
            }
        }

        private void ClearComparison_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ComparisonViewModel;
            viewModel?.ClearComparison();
        }
    }
}