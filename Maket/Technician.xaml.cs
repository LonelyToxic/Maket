using System;
using System.Windows;

namespace Maket
{
    public partial class Technician : Window
    {
        public Technician()
        {
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление настройками");
        }

        private void NetworkStatus_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Состояние сети");
        }

        private void MaintenanceLog_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Журнал технического обслуживания");
        }

        private void Equipment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление оборудованием");
        }
    }
}
