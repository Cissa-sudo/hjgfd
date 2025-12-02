using System.Windows;
using System.Text.RegularExpressions;
using SpaApp.Repositories;
using SpaApp.Models;
using System.Globalization;

namespace SpaApp
{
    public partial class RegisterWindow : Window
    {
        public bool IsSuccess { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        private readonly UserRepository _userRepository;

        public RegisterWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ResetErrorStyles();

            bool hasErrors = false;
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowError(txtFullName, "Введите полное имя");
                hasErrors = true;
            }
            else if (txtFullName.Text.Length < 2)
            {
                ShowError(txtFullName, "Имя должно содержать минимум 2 символа");
                hasErrors = true;
            }
            else if (txtFullName.Text.Length > 50)
            {
                ShowError(txtFullName, "Имя не должно превышать 50 символов");
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowError(txtEmail, "Введите email");
                hasErrors = true;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                ShowError(txtEmail, "Введите корректный email на английском языке (пример: user@example.com)");
                hasErrors = true;
            }
            else if (txtEmail.Text.Length > 100)
            {
                ShowError(txtEmail, "Email не должен превышать 100 символов");
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError(txtUsername, "Введите имя пользователя");
                hasErrors = true;
            }
            else if (txtUsername.Text.Length < 3)
            {
                ShowError(txtUsername, "Имя пользователя должно содержать минимум 3 символа");
                hasErrors = true;
            }
            else if (txtUsername.Text.Length > 20)
            {
                ShowError(txtUsername, "Имя пользователя не должно превышать 20 символов");
                hasErrors = true;
            }
            else if (!Regex.IsMatch(txtUsername.Text, @"^[a-zA-Z0-9_]+$"))
            {
                ShowError(txtUsername, "Имя пользователя может содержать только латинские буквы, цифры и символ подчеркивания");
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                ShowError(txtPassword, "Введите пароль");
                hasErrors = true;
            }
            else if (txtPassword.Password.Length < 4)
            {
                ShowError(txtPassword, "Пароль должен содержать минимум 4 символа");
                hasErrors = true;
            }
            else if (txtPassword.Password.Length > 50)
            {
                ShowError(txtPassword, "Пароль не должен превышать 50 символов");
                hasErrors = true;
            }

            if (hasErrors)
                return;

            if (!Database.DatabaseHelper.TestConnection())
            {
                MessageBox.Show("Не удалось подключиться к базе данных. Проверьте настройки подключения.", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_userRepository.UserExists(txtUsername.Text))
            {
                ShowError(txtUsername, "Пользователь с таким именем уже существует");
                return;
            }

            if (_userRepository.EmailExists(txtEmail.Text))
            {
                ShowError(txtEmail, "Пользователь с таким email уже существует");
                return;
            }

         var user = new User
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Password,
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim().ToLower()
            };

            if (_userRepository.CreateUser(user))
            {
                Username = user.Username;
                FullName = user.FullName;
                Email = user.Email;
                IsSuccess = true;

                MessageBox.Show($"Регистрация успешно завершена!\n\nДобро пожаловать, {user.FullName}!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации пользователя. Попробуйте позже.", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return false;

                if (!Regex.IsMatch(email, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$"))
                    return false;

                    foreach (char c in email)
                {
                         if (c > 127)
                        return false;
                }

     
                string[] parts = email.Split('@');
                if (parts.Length != 2)
                    return false;

                string localPart = parts[0];
                string domainPart = parts[1];

                if (string.IsNullOrEmpty(localPart) || localPart.StartsWith(".") || localPart.EndsWith("."))
                    return false;

                           if (string.IsNullOrEmpty(domainPart) || domainPart.StartsWith("-") || domainPart.EndsWith("-") || domainPart.StartsWith(".") || domainPart.EndsWith("."))
                    return false;

                    if (email.Contains("..") || email.Contains(".-") || email.Contains("-."))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(System.Windows.Controls.Control control, string message)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Red;
            control.BorderThickness = new Thickness(2);

            MessageBox.Show(message, "Ошибка ввода",
                          MessageBoxButton.OK, MessageBoxImage.Warning);

            control.Focus();
        }

        private void ResetErrorStyles()
        {
            txtFullName.BorderBrush = System.Windows.Media.Brushes.DarkGray;
            txtEmail.BorderBrush = System.Windows.Media.Brushes.DarkGray;
            txtUsername.BorderBrush = System.Windows.Media.Brushes.DarkGray;
            txtPassword.BorderBrush = System.Windows.Media.Brushes.DarkGray;

            txtFullName.BorderThickness = new Thickness(1);
            txtEmail.BorderThickness = new Thickness(1);
            txtUsername.BorderThickness = new Thickness(1);
            txtPassword.BorderThickness = new Thickness(1);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

   private void txtFullName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ResetFieldErrorStyle(txtFullName);
        }

        private void txtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ResetFieldErrorStyle(txtEmail);
        }

        private void txtUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ResetFieldErrorStyle(txtUsername);
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ResetFieldErrorStyle(txtPassword);
        }

        private void ResetFieldErrorStyle(System.Windows.Controls.Control control)
        {
            control.BorderBrush = System.Windows.Media.Brushes.DarkGray;
            control.BorderThickness = new Thickness(1);
        }
    }
}
