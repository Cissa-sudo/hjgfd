using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using SpaApp.Models;

namespace SpaApp.ViewModels
{
    public class ComparisonViewModel : INotifyPropertyChanged
    {
        private List<Subscription> _subscriptions;
        private MainViewModel _mainViewModel;

        public List<Subscription> Subscriptions
        {
            get => _subscriptions;
            private set
            {
                _subscriptions = value;
                OnPropertyChanged(nameof(Subscriptions));
                OnPropertyChanged(nameof(HasThirdSubscription));
            }
        }

        public bool HasThirdSubscription => Subscriptions.Count >= 3;

        public ComparisonViewModel(List<Subscription> subscriptions, MainViewModel mainViewModel)
        {
            Subscriptions = new List<Subscription>(subscriptions);
            _mainViewModel = mainViewModel;
        }

        public void RemoveFromComparison(Subscription subscription)
        {
            if (subscription != null && Subscriptions.Contains(subscription))
            {
                var updatedSubscriptions = new List<Subscription>(Subscriptions);
                updatedSubscriptions.Remove(subscription);
                Subscriptions = updatedSubscriptions; // Это вызовет PropertyChanged

                _mainViewModel.RemoveFromComparisonDirectly(subscription);

                // Закрываем окно если осталось меньше 2 абонементов
                if (Subscriptions.Count < 2)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is ComparisonWindow comparisonWindow && comparisonWindow.DataContext == this)
                            {
                                comparisonWindow.Close();
                                break;
                            }
                        }
                    });
                }
            }
        }

        public void ClearComparison()
        {
            Subscriptions = new List<Subscription>(); // Очищаем коллекцию
            _mainViewModel.ClearComparisonDirectly();

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is ComparisonWindow comparisonWindow && comparisonWindow.DataContext == this)
                    {
                        comparisonWindow.Close();
                        break;
                    }
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}