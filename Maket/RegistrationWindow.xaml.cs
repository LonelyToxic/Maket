using System.Windows;
using System.Windows.Controls;

namespace Maket
{
    public partial class RegistrationWindow : Window
    {
        private DatabaseHelper dbHelper;

        public RegistrationWindow()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string userType = ((ComboBoxItem)cbUserType.SelectedItem).Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Имя пользователя и пароль не могут быть пустыми");
                return;
            }

            dbHelper.RegisterUser(username, password, userType);
            MessageBox.Show("Пользователь зарегистрирован");
            this.Close();
        }
    }
}
