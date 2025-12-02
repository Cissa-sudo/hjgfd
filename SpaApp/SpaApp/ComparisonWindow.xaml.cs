using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;
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

            // Подписываемся на событие изменения свойств ViewModel
            var viewModel = DataContext as ComparisonViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // При изменении коллекции подписок принудительно обновляем привязки
            if (e.PropertyName == nameof(ComparisonViewModel.Subscriptions) ||
                e.PropertyName == nameof(ComparisonViewModel.HasThirdSubscription))
            {
                // Обновляем ItemsControl
                var itemsControl = FindName("SubscriptionItemsControl") as ItemsControl;
                itemsControl?.GetBindingExpression(ItemsControl.ItemsSourceProperty)?.UpdateTarget();
            }
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

                // Обновляем привязки данных
                this.UpdateLayout();
            }
        }

        private void ClearComparison_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ComparisonViewModel;
            viewModel?.ClearComparison();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Отписываемся от события при закрытии окна
            var viewModel = DataContext as ComparisonViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
            base.OnClosing(e);
        }
    }
}