using System;
using System.Collections.Generic;
using System.Windows;
using SpaApp.Models;

namespace SpaApp.ViewModels
{
    public class ComparisonViewModel
    {
        public List<Subscription> Subscriptions { get; }
        public bool HasThirdSubscription => Subscriptions.Count >= 3;
        private MainViewModel _mainViewModel;

        public ComparisonViewModel(List<Subscription> subscriptions, MainViewModel mainViewModel)
        {
            Subscriptions = new List<Subscription>(subscriptions);
            _mainViewModel = mainViewModel;
        }

        public void RemoveFromComparison(Subscription subscription)
        {
            if (subscription != null && Subscriptions.Contains(subscription))
            {
                Subscriptions.Remove(subscription);
                _mainViewModel.RemoveFromComparisonDirectly(subscription);

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
    }
}
