using System.Windows;
using Maket.Models;

namespace Maket
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;

        public MainWindow()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Имя пользователя и пароль не могут быть пустыми");
                return;
            }

            ValidationResult result = dbHelper.ValidateUser(username, password);

            if (result.IsValid)
            {
                if (result.UserType == "Администратор")
                {
                    Admin adminWindow = new Admin();
                    adminWindow.Show();
                }
                else if (result.UserType == "Клиент" && result.UserId.HasValue)
                {
                    ClientWindow clientWindow = new ClientWindow(result.UserId.Value);
                    clientWindow.Show();
                }
                else
                {
                    MessageBox.Show("Неизвестный тип пользователя или отсутствует UserID");
                    return;
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функциональность восстановления пароля пока не реализована.");
        }
    }
}
