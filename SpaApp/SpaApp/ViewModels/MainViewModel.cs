using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SpaApp.Models;
using SpaApp.Repositories;
using SpaApp.Commands;

namespace SpaApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private string _searchText;
        private int _selectedTypeId;
        private decimal _minPrice;
        private decimal _maxPrice = 10000;
        private double _minRating;
        private string _selectedSortOption = "name_asc";
        private readonly SubscriptionRepository _subscriptionRepository;
        private readonly UserRepository _userRepository;
        private List<Subscription> _allSubscriptions;
        private readonly ObservableCollection<Subscription> _comparisonList = new ObservableCollection<Subscription>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        public bool IsLoggedIn => CurrentUser != null;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value != null && value.Length > 250)
                    value = value.Substring(0, 250);

                _searchText = value;
                OnPropertyChanged();
            }
        }

        public int SelectedTypeId
        {
            get => _selectedTypeId;
            set
            {
                _selectedTypeId = value;
                OnPropertyChanged();
            }
        }

        public decimal MinPrice
        {
            get => _minPrice;
            set
            {
                _minPrice = value < 0 ? 0 : value;
                OnPropertyChanged();
            }
        }

        public decimal MaxPrice
        {
            get => _maxPrice;
            set
            {
                _maxPrice = value < 0 ? 0 : value;
                OnPropertyChanged();
            }
        }

        public double MinRating
        {
            get => _minRating;
            set
            {
                _minRating = value;
                OnPropertyChanged();
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
            }
        }

        public int SelectedComparisonCount => _comparisonList.Count;

        public bool CanCompare => _comparisonList.Count >= 2;

        public ObservableCollection<Subscription> Subscriptions { get; } = new ObservableCollection<Subscription>();
        public ObservableCollection<Subscription> ComparisonList => _comparisonList;
        public ObservableCollection<SubscriptionType> SubscriptionTypes { get; } = new ObservableCollection<SubscriptionType>();

        public Dictionary<string, string> SortOptions { get; } = new Dictionary<string, string>
        {
            {"По названию (А-Я)", "name_asc"},
            {"По названию (Я-А)", "name_desc"},
            {"Сначала дешевые", "price_asc"},
            {"Сначала дорогие", "price_desc"},
            {"По рейтингу (высокий)", "rating_desc"},
            {"По рейтингу (низкий)", "rating_asc"}
        };

        public Dictionary<string, double> RatingOptions { get; } = new Dictionary<string, double>
        {
            {"Любой рейтинг", 0},
            {"⭐ 4.0 и выше", 4.0},
            {"⭐ 4.5 и выше", 4.5},
            {"⭐ 5.0", 5.0}
        };

        public bool HasSubscriptions => Subscriptions.Any();

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand CompareCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ResetFiltersCommand { get; }
        public ICommand AddToComparisonCommand { get; }
        public ICommand RemoveFromComparisonCommand { get; }
        public ICommand ClearComparisonCommand { get; }

        public MainViewModel()
        {
            _subscriptionRepository = new SubscriptionRepository();
            _userRepository = new UserRepository();

            LoadSubscriptionTypes();
            LoadSubscriptions();

            LoginCommand = new RelayCommand(_ => ShowLogin());
            LogoutCommand = new RelayCommand(_ => Logout());
            CompareCommand = new RelayCommand(_ => CompareSubscriptions(), _ => CanCompare);
            SearchCommand = new RelayCommand(_ => ApplyFilters());
            ResetFiltersCommand = new RelayCommand(_ => ResetFilters());
            AddToComparisonCommand = new RelayCommand(AddToComparison);
            RemoveFromComparisonCommand = new RelayCommand(RemoveFromComparison);
            ClearComparisonCommand = new RelayCommand(_ => ClearComparison());
        }

        private void LoadSubscriptions()
        {
            try
            {
                _allSubscriptions = _subscriptionRepository.GetAllSubscriptions();

                // Инициализируем флаги сравнения для всех подписок
                foreach (var subscription in _allSubscriptions)
                {
                    subscription.IsInComparison = false; // По умолчанию все не в сравнении
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке абонементов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSubscriptionTypes()
        {
            try
            {
                var types = _subscriptionRepository.GetSubscriptionTypes();
                SubscriptionTypes.Clear();
                foreach (var type in types)
                {
                    SubscriptionTypes.Add(type);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов абонементов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_allSubscriptions == null) return;

            try
            {
                var filtered = _allSubscriptions.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchLower = SearchText.ToLower();
                    filtered = filtered.Where(s =>
                        (s.Name?.ToLower().Contains(searchLower) ?? false) ||
                        (s.FacilityName?.ToLower().Contains(searchLower) ?? false) ||
                        (s.Description?.ToLower().Contains(searchLower) ?? false));
                }

                if (SelectedTypeId > 0)
                {
                    var selectedType = SubscriptionTypes.FirstOrDefault(t => t.Id == SelectedTypeId);
                    if (selectedType != null)
                    {
                        filtered = filtered.Where(s => s.TypeName == selectedType.Name);
                    }
                }

                filtered = filtered.Where(s => s.Price >= MinPrice && s.Price <= MaxPrice);

                if (MinRating > 0)
                {
                    filtered = filtered.Where(s => s.Rating >= MinRating);
                }

                switch (SelectedSortOption)
                {
                    case "name_asc":
                        filtered = filtered.OrderBy(s => s.Name);
                        break;
                    case "name_desc":
                        filtered = filtered.OrderByDescending(s => s.Name);
                        break;
                    case "price_asc":
                        filtered = filtered.OrderBy(s => s.Price);
                        break;
                    case "price_desc":
                        filtered = filtered.OrderByDescending(s => s.Price);
                        break;
                    case "rating_desc":
                        filtered = filtered.OrderByDescending(s => s.Rating);
                        break;
                    case "rating_asc":
                        filtered = filtered.OrderBy(s => s.Rating);
                        break;
                }

                Subscriptions.Clear();
                foreach (var subscription in filtered)
                {
                    // Обновляем флаг сравнения для отображаемых подписок
                    subscription.IsInComparison = _comparisonList.Contains(subscription);
                    Subscriptions.Add(subscription);
                }

                OnPropertyChanged(nameof(HasSubscriptions));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при применении фильтров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetFilters()
        {
            SearchText = string.Empty;
            SelectedTypeId = 0;
            MinPrice = 0;
            MaxPrice = 10000;
            MinRating = 0;
            SelectedSortOption = "name_asc";
            ApplyFilters();
        }

        private void AddToComparison(object parameter)
        {
            if (parameter is Subscription subscription)
            {
                if (_comparisonList.Contains(subscription))
                {
                    _comparisonList.Remove(subscription);
                    subscription.IsInComparison = false;
                    MessageBox.Show($"{subscription.Name} удален из сравнения",
                        "Сравнение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (_comparisonList.Count >= 3)
                    {
                        MessageBox.Show("Можно сравнивать не более 3 абонементов",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    _comparisonList.Add(subscription);
                    subscription.IsInComparison = true;
                    MessageBox.Show($"{subscription.Name} добавлен к сравнению",
                        "Сравнение", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                UpdateComparisonProperties();

                // Принудительно обновляем отображение для этой подписки
                RefreshSubscriptionDisplay(subscription);
            }
        }

        private void RefreshSubscriptionDisplay(Subscription subscription)
        {
            // Находим индекс подписки в коллекции
            var index = Subscriptions.IndexOf(subscription);
            if (index >= 0)
            {
                // Временно удаляем и добавляем обратно для обновления привязки
                Subscriptions.RemoveAt(index);
                Subscriptions.Insert(index, subscription);
            }
        }

        private void RemoveFromComparison(object parameter)
        {
            if (parameter is Subscription subscription)
            {
                _comparisonList.Remove(subscription);
                subscription.IsInComparison = false;
                MessageBox.Show($"{subscription.Name} удален из сравнения",
                    "Сравнение", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateComparisonProperties();
                RefreshSubscriptionDisplay(subscription);
            }
        }

        private void ClearComparison()
        {
            if (_comparisonList.Count > 0)
            {
                var result = MessageBox.Show("Вы уверены, что хотите очистить список сравнения?",
                    "Очистка сравнения", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var subscriptionsToUpdate = new List<Subscription>(_comparisonList);

                    foreach (var subscription in subscriptionsToUpdate)
                    {
                        subscription.IsInComparison = false;
                        RefreshSubscriptionDisplay(subscription);
                    }

                    _comparisonList.Clear();
                    MessageBox.Show("Список сравнения очищен",
                        "Сравнение", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateComparisonProperties();
                }
            }
            else
            {
                MessageBox.Show("Список сравнения уже пуст",
                    "Сравнение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void RemoveFromComparisonDirectly(Subscription subscription)
        {
            if (_comparisonList.Contains(subscription))
            {
                _comparisonList.Remove(subscription);
                subscription.IsInComparison = false;
                UpdateComparisonProperties();
                RefreshSubscriptionDisplay(subscription);

                if (_comparisonList.Count >= 2)
                {
                    MessageBox.Show($"{subscription.Name} удален из сравнения",
                        "Сравнение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public void ClearComparisonDirectly()
        {
            if (_comparisonList.Count > 0)
            {
                var subscriptionsToUpdate = new List<Subscription>(_comparisonList);

                foreach (var subscription in subscriptionsToUpdate)
                {
                    subscription.IsInComparison = false;
                    RefreshSubscriptionDisplay(subscription);
                }

                _comparisonList.Clear();
                UpdateComparisonProperties();
                MessageBox.Show("Список сравнения очищен",
                    "Сравнение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateComparisonProperties()
        {
            OnPropertyChanged(nameof(SelectedComparisonCount));
            OnPropertyChanged(nameof(CanCompare));
            OnPropertyChanged(nameof(ComparisonList));
        }

        private void CompareSubscriptions()
        {
            if (_comparisonList.Count < 2)
            {
                MessageBox.Show("Выберите как минимум 2 абонемента для сравнения",
                    "Сравнение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var comparisonWindow = new ComparisonWindow(new List<Subscription>(_comparisonList), this);
                comparisonWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии сравнения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowLogin()
        {
            try
            {
                var loginWindow = new LoginWindow();
                var result = loginWindow.ShowDialog();

                if (result == true && loginWindow.IsSuccess)
                {
                    var user = _userRepository.GetUserByUsername(loginWindow.Username);
                    if (user != null)
                    {
                        CurrentUser = user;
                        MessageBox.Show($"Добро пожаловать, {user.FullName}!",
                            "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        CurrentUser = new User
                        {
                            Username = loginWindow.Username,
                            FullName = loginWindow.FullName,
                            Email = loginWindow.Email
                        };
                        MessageBox.Show($"Добро пожаловать, {loginWindow.FullName}!",
                            "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout()
        {
            if (CurrentUser != null)
            {
                MessageBox.Show($"До свидания, {CurrentUser.FullName}!",
                    "Выход", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            CurrentUser = null;

            var subscriptionsToUpdate = new List<Subscription>(_comparisonList);
            foreach (var subscription in subscriptionsToUpdate)
            {
                subscription.IsInComparison = false;
            }

            _comparisonList.Clear();
            UpdateComparisonProperties();

            // Обновляем отображение всех подписок
            ApplyFilters();
        }
    }
}