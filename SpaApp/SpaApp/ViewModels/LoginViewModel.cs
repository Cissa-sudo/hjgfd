using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SpaApp.Commands;
using SpaApp.Models;
using SpaApp.Repositories;

namespace SpaApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private readonly UserRepository _userRepository;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand GuestLoginCommand { get; }

        public LoginViewModel()
        {
            _userRepository = new UserRepository();
            LoginCommand = new RelayCommand(ExecuteLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister);
            GuestLoginCommand = new RelayCommand(ExecuteGuestLogin);
        }

        private void ExecuteLogin(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Введите имя пользователя и пароль", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_userRepository.ValidateUser(Username, Password))
            {
                var user = _userRepository.GetUserByUsername(Username);
                if (user != null)
                {
               
                    CurrentUser = user;
                    MessageBox.Show($"Добро пожаловать, {user.FullName}!", "Успешный вход",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка входа",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecuteRegister(object parameter)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }

        private void ExecuteGuestLogin(object parameter)
        {
            CurrentUser = new User { Username = "Гость", FullName = "Гостевой пользователь", Role = "Guest" };
            MessageBox.Show("Добро пожаловать, Гость!", "Гостевой вход",
                          MessageBoxButton.OK, MessageBoxImage.Information);
         
        }
        
        public User CurrentUser { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
