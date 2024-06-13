using System;
using System.Windows;

namespace Maket
{
    public partial class Manager : Window
    {
        public Manager()
        {
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление настройками");
        }

        private void Sessions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление сессиями");
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Отчеты");
        }

        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление клиентами");
        }
    }
}
