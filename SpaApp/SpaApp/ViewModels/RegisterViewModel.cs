using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SpaApp.Commands;
using SpaApp.Models;
using SpaApp.Repositories;

namespace SpaApp.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _fullName;
        private string _email;
        private string _username;
        private string _password;
        private string _confirmPassword;
        private readonly UserRepository _userRepository;

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand BackCommand { get; }

        public RegisterViewModel()
        {
            _userRepository = new UserRepository();
            RegisterCommand = new RelayCommand(ExecuteRegister, CanExecuteRegister);
            BackCommand = new RelayCommand(ExecuteBack);
        }

        private void ExecuteRegister(object parameter)
        {
            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_userRepository.UserExists(Username))
            {
                MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = new User
            {
                FullName = FullName,
                Email = Email,
                Username = Username,
                Password = Password, 
                Role = "User"
            };

            if (_userRepository.Register(user))
            {
                MessageBox.Show("Регистрация успешно завершена!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                ExecuteBack(null);
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteRegister(object parameter)
        {
            return !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private void ExecuteBack(object parameter)
        {
                  foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginWindow)
                {
                    window.Show();
                    break;
                }
            }

            foreach (Window window in Application.Current.Windows)
            {
                if (window is RegisterWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
