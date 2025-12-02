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
        private bool _isRegistrationSuccessful;

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFormValid));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFormValid));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFormValid));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFormValid));
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFormValid));
                OnPropertyChanged(nameof(PasswordsMatch));
            }
        }

        public bool PasswordsMatch => Password == ConfirmPassword;
        public bool IsFormValid => !string.IsNullOrWhiteSpace(FullName) &&
                                 !string.IsNullOrWhiteSpace(Email) &&
                                 !string.IsNullOrWhiteSpace(Username) &&
                                 !string.IsNullOrWhiteSpace(Password) &&
                                 !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                                 PasswordsMatch;

        public bool IsRegistrationSuccessful
        {
            get => _isRegistrationSuccessful;
            private set
            {
                _isRegistrationSuccessful = value;
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
            IsRegistrationSuccessful = false;
        }

        private void ExecuteRegister(object parameter)
        {
            // Дополнительная проверка перед регистрацией
            if (!PasswordsMatch)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                IsRegistrationSuccessful = false;
                return;
            }

            if (_userRepository.UserExists(Username))
            {
                MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                IsRegistrationSuccessful = false;
                return;
            }

            // Проверка валидности email
            if (!IsValidEmail(Email))
            {
                MessageBox.Show("Пожалуйста, введите корректный email адрес", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                IsRegistrationSuccessful = false;
                return;
            }

            var user = new User
            {
                FullName = FullName,
                Email = Email,
                Username = Username,
                Password = Password, // В реальном приложении нужно хешировать!
                Role = "User"
            };

            if (_userRepository.Register(user))
            {
                MessageBox.Show("Регистрация успешно завершена!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                IsRegistrationSuccessful = true;
                ExecuteBack(null);
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                IsRegistrationSuccessful = false;
            }
        }

        private bool CanExecuteRegister(object parameter)
        {
            return IsFormValid;
        }

        private void ExecuteBack(object parameter)
        {
            // Логика возврата к окну входа
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginWindow)
                {
                    window.Show();
                    break;
                }
            }

            // Закрываем текущее окно регистрации
            foreach (Window window in Application.Current.Windows)
            {
                if (window is RegisterWindow registerWindow)
                {
                    registerWindow.Close();
                    break;
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}