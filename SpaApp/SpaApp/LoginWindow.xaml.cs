using System.Windows;
using SpaApp.Repositories;

namespace SpaApp
{
    public partial class LoginWindow : Window
    {
        public bool IsSuccess { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        private readonly UserRepository _userRepository;

        public LoginWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Введите имя пользователя и пароль", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем подключение к БД
            if (!Database.DatabaseHelper.TestConnection())
            {
                MessageBox.Show("Не удалось подключиться к базе данных. Проверьте настройки подключения.", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверяем пользователя в БД
            if (_userRepository.ValidateUser(txtUsername.Text, txtPassword.Password))
            {
                var user = _userRepository.GetUserByUsername(txtUsername.Text);
                if (user != null)
                {
                    Username = user.Username;
                    FullName = user.FullName;
                    Email = user.Email;
                }
                else
                {
                    Username = txtUsername.Text;
                    FullName = txtUsername.Text;
                }

                IsSuccess = true;
                this.DialogResult = true;
                this.Close(); // Закрываем окно входа после успешного входа
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка входа",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно регистрации поверх окна входа
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = this; // Устанавливаем владельца
            registerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var registerResult = registerWindow.ShowDialog();

            if (registerResult == true && registerWindow.IsSuccess)
            {
                // Если регистрация успешна, автоматически входим и закрываем окно входа
                Username = registerWindow.Username;
                FullName = registerWindow.FullName;
                Email = registerWindow.Email;
                IsSuccess = true;

                this.DialogResult = true;
                this.Close(); // Закрываем окно входа
            }
            // Если регистрация отменена, остаемся в окне входа (ничего не делаем)
        }

        private void GuestLoginButton_Click(object sender, RoutedEventArgs e)
        {
            Username = "Гость";
            FullName = "Гостевой пользователь";
            Email = "guest@example.com";
            IsSuccess = true;
            this.DialogResult = true;
            this.Close(); // Закрываем окно входа
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close(); // Закрываем окно входа
        }
    }
}