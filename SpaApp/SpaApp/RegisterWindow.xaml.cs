using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SpaApp.ViewModels;

namespace SpaApp
{
    public partial class RegisterWindow : Window
    {
        private RegisterViewModel _viewModel;

        // Публичные свойства для доступа из других окон
        public bool IsSuccess { get; private set; }
        public string Username => _viewModel?.Username;
        public string FullName => _viewModel?.FullName;
        public string Email => _viewModel?.Email;

        public RegisterWindow()
        {
            InitializeComponent();
            _viewModel = new RegisterViewModel();
            DataContext = _viewModel;
            IsSuccess = false;

            // Подписываемся на события
            txtFullName.TextChanged += TxtFullName_TextChanged;
            txtEmail.TextChanged += TxtEmail_TextChanged;
            txtUsername.TextChanged += TxtUsername_TextChanged;
            txtPassword.PasswordChanged += TxtPassword_PasswordChanged;
            txtConfirmPassword.PasswordChanged += TxtConfirmPassword_PasswordChanged;
            btnRegister.Click += RegisterButton_Click;
            btnBack.Click += BackButton_Click;
        }

        private void TxtFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.FullName = txtFullName.Text;
            }
        }

        private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Email = txtEmail.Text;
            }
        }

        private void TxtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Username = txtUsername.Text;
            }
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Password = txtPassword.Password;
            }
        }

        private void TxtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.ConfirmPassword = txtConfirmPassword.Password;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем границы полей перед проверкой
            ResetFieldBorders();

            bool hasErrors = false;
            string errorMessage = "";

            // Проверка заполнения всех полей
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                HighlightErrorField(txtFullName);
                errorMessage += "• Поле 'Полное имя' обязательно для заполнения\n";
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                HighlightErrorField(txtEmail);
                errorMessage += "• Поле 'Email' обязательно для заполнения\n";
                hasErrors = true;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                HighlightErrorField(txtEmail);
                errorMessage += "• Пожалуйста, введите корректный email адрес\n";
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                HighlightErrorField(txtUsername);
                errorMessage += "• Поле 'Имя пользователя' обязательно для заполнения\n";
                hasErrors = true;
            }
            else
            {
                // Проверка на допустимые символы в имени пользователя
                foreach (char c in txtUsername.Text)
                {
                    if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
                    {
                        HighlightErrorField(txtUsername);
                        errorMessage += "• Имя пользователя может содержать только буквы, цифры, символы подчеркивания и дефисы\n";
                        hasErrors = true;
                        break;
                    }
                }

                // Проверка минимальной длины имени пользователя
                if (txtUsername.Text.Length < 3)
                {
                    HighlightErrorField(txtUsername);
                    errorMessage += "• Имя пользователя должно содержать минимум 3 символа\n";
                    hasErrors = true;
                }
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                HighlightErrorField(txtPassword);
                errorMessage += "• Поле 'Пароль' обязательно для заполнения\n";
                hasErrors = true;
            }
            else if (txtPassword.Password.Length < 6)
            {
                HighlightErrorField(txtPassword);
                errorMessage += "• Пароль должен содержать минимум 6 символов\n";
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Password))
            {
                HighlightErrorField(txtConfirmPassword);
                errorMessage += "• Поле 'Подтвердите пароль' обязательно для заполнения\n";
                hasErrors = true;
            }
            else if (txtPassword.Password != txtConfirmPassword.Password)
            {
                HighlightErrorField(txtConfirmPassword);
                errorMessage += "• Пароли не совпадают\n";
                hasErrors = true;
            }

            // Если есть ошибки, показываем сообщение и прерываем регистрацию
            if (hasErrors)
            {
                MessageBox.Show(errorMessage.Trim(), "Ошибки в форме", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Если все проверки пройдены, вызываем команду регистрации из ViewModel
            if (_viewModel.RegisterCommand.CanExecute(null))
            {
                _viewModel.RegisterCommand.Execute(null);
                // Устанавливаем флаг успешной регистрации
                IsSuccess = _viewModel.IsRegistrationSuccessful;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Вызов команды возврата из ViewModel
            _viewModel.BackCommand.Execute(null);
        }

        // Метод для валидации email
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

        // Метод для подсветки поля с ошибкой
        private void HighlightErrorField(Control control)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Red;
            control.BorderThickness = new Thickness(2);
        }

        // Метод для сброса границ полей
        private void ResetFieldBorders()
        {
            var defaultBrush = System.Windows.Media.Brushes.LightGray;
            var defaultThickness = new Thickness(1);

            txtFullName.BorderBrush = defaultBrush;
            txtFullName.BorderThickness = defaultThickness;

            txtEmail.BorderBrush = defaultBrush;
            txtEmail.BorderThickness = defaultThickness;

            txtUsername.BorderBrush = defaultBrush;
            txtUsername.BorderThickness = defaultThickness;

            txtPassword.BorderBrush = defaultBrush;
            txtPassword.BorderThickness = defaultThickness;

            txtConfirmPassword.BorderBrush = defaultBrush;
            txtConfirmPassword.BorderThickness = defaultThickness;
        }
    }
}