using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Maket.Models;

namespace Maket
{
    public partial class Admin : Window
    {
        private int clientId; // Добавлено
        private DatabaseHelper dbHelper;
        private Booking selectedBooking;
        private Client selectedClient;
        private Computer selectedComputer;
        private Game selectedGame;
        private FoodOrDrink selectedFoodOrDrink;

        public Admin()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            LoadData();
        }

        private void LoadData()
        {
            var clients = dbHelper.GetClients();
            ClientsDataGrid.ItemsSource = clients;

            var computers = dbHelper.GetComputers();
            ComputersDataGrid.ItemsSource = computers;

            var bookings = dbHelper.GetBookings();
            BookingsDataGrid.ItemsSource = bookings;

            var games = dbHelper.GetGames();
            GamesDataGrid.ItemsSource = games;

            var foodAndDrinks = dbHelper.GetFoodAndDrinks();
            FoodAndDrinksDataGrid.ItemsSource = foodAndDrinks;
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            ShowClients();
        }

        private void Computers_Click(object sender, RoutedEventArgs e)
        {
            ShowComputers();
        }

        private void Games_Click(object sender, RoutedEventArgs e)
        {
            ShowGames();
        }

        private void Reservation_Click(object sender, RoutedEventArgs e)
        {
            ShowBookings();
        }

        private void FoodAndDrinks_Click(object sender, RoutedEventArgs e)
        {
            ShowFoodAndDrinks();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление настройками");
        }

        private void Cart_Click(object sender, RoutedEventArgs e)
        {
            ShowCart();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedItem == null || ComputerNumberComboBox.SelectedItem == null || DatePicker.SelectedDate == null || StartTimePicker.Value == null || EndTimePicker.Value == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            int clientId = (int)ClientComboBox.SelectedValue;
            int computerNumber = (int)ComputerNumberComboBox.SelectedValue;
            DateTime date = DatePicker.SelectedDate.Value;
            string startTime = StartTimePicker.Value.Value.ToString("hh\\:mm");
            string endTime = EndTimePicker.Value.Value.ToString("hh\\:mm");

            var booking = new Booking
            {
                ClientID = clientId,
                ComputerNumber = computerNumber,
                BookingDate = date,
                StartTime = TimeSpan.Parse(startTime),  // Исправлено
                EndTime = TimeSpan.Parse(endTime),  // Исправлено
                Cost = CalculateBookingCost(new Booking { StartTime = TimeSpan.Parse(startTime), EndTime = TimeSpan.Parse(endTime) }),
            };

            dbHelper.AddToCart(booking);
            MessageBox.Show("Бронирование добавлено в корзину");
            LoadCart();
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            var cartItems = dbHelper.GetCartItems(clientId);  // Исправлено
            dbHelper.CompletePurchase(cartItems);
            MessageBox.Show("Оплата прошла успешно");
            LoadCart();
        }

        private void AddFoodOrDrink_Click(object sender, RoutedEventArgs e)
        {
            string name = FoodNameTextBox.Text;
            decimal price = decimal.Parse(FoodPriceTextBox.Text);
            int quantity = int.Parse(FoodQuantityTextBox.Text);
            string imagePath = FoodImageTextBox.Text;

            dbHelper.AddFoodOrDrink(name, price, imagePath, quantity);
            MessageBox.Show("Еда или напиток добавлены");
            LoadFoodAndDrinks();
            ClearFoodOrDrinkFormFields();
        }

        private void EditReservation_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBooking != null)
            {
                int clientId = (int)ClientComboBox.SelectedValue;
                int computerNumber = (int)ComputerNumberComboBox.SelectedValue;
                DateTime date = DatePicker.SelectedDate.HasValue ? DatePicker.SelectedDate.Value : DateTime.Now;
                string startTime = StartTimePicker.Value.HasValue ? StartTimePicker.Value.Value.ToString("hh\\:mm") : "00:00";
                string endTime = EndTimePicker.Value.HasValue ? EndTimePicker.Value.Value.ToString("hh\\:mm") : "00:00";

                dbHelper.UpdateBooking(selectedBooking.BookingID, clientId, computerNumber, date, TimeSpan.Parse(startTime), TimeSpan.Parse(endTime));
                MessageBox.Show("Бронирование обновлено");
                LoadBookings();
                ClearReservationFormFields();
            }
        }

        private void DeleteReservation_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBooking != null)
            {
                dbHelper.DeleteBooking(selectedBooking.BookingID);
                MessageBox.Show("Бронирование удалено");
                LoadBookings();
                ClearReservationFormFields();
            }
        }

        private void ClientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem != null)
            {
                selectedClient = (Client)ClientsDataGrid.SelectedItem;
                EditClientButton.IsEnabled = true;
                DeleteClientButton.IsEnabled = true;

                ClientNameTextBox.Text = selectedClient.FullName;
                ClientAgeTextBox.Text = selectedClient.Age.ToString();
                ClientContactTextBox.Text = selectedClient.ContactInfo;
            }
            else
            {
                EditClientButton.IsEnabled = false;
                DeleteClientButton.IsEnabled = false;
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            string name = ClientNameTextBox.Text;
            int age = int.Parse(ClientAgeTextBox.Text);
            string contactInfo = ClientContactTextBox.Text;

            dbHelper.AddClient(name, age, contactInfo);
            MessageBox.Show("Клиент добавлен");
            LoadClients();
            ClearClientFormFields();
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (selectedClient != null)
            {
                string name = ClientNameTextBox.Text;
                int age = int.Parse(ClientAgeTextBox.Text);
                string contactInfo = ClientContactTextBox.Text;

                dbHelper.UpdateClient(selectedClient.ClientID, name, age, contactInfo);
                MessageBox.Show("Клиент обновлен");
                LoadClients();
                ClearClientFormFields();
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (selectedClient != null)
            {
                dbHelper.DeleteClient(selectedClient.ClientID);
                MessageBox.Show("Клиент удален");
                LoadClients();
                ClearClientFormFields();
            }
        }

        private void ClearReservationFormFields()
        {
            ClientComboBox.SelectedIndex = -1;
            ComputerNumberComboBox.SelectedIndex = -1;
            DatePicker.SelectedDate = null;
            StartTimePicker.Value = null;
            EndTimePicker.Value = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private void ClearClientFormFields()
        {
            ClientNameTextBox.Text = string.Empty;
            ClientAgeTextBox.Text = string.Empty;
            ClientContactTextBox.Text = string.Empty;
            EditClientButton.IsEnabled = false;
            DeleteClientButton.IsEnabled = false;
        }

        private void ComputersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComputersDataGrid.SelectedItem != null)
            {
                selectedComputer = (Computer)ComputersDataGrid.SelectedItem;
                EditComputerButton.IsEnabled = true;
                DeleteComputerButton.IsEnabled = true;

                ComputerNumberTextBox.Text = selectedComputer.ComputerNumber.ToString();
                ComputerStatusTextBox.Text = selectedComputer.Status;
                ComputerProcessorTextBox.Text = selectedComputer.Processor;
                ComputerGraphicsCardTextBox.Text = selectedComputer.GraphicsCard;
                ComputerRAMTextBox.Text = selectedComputer.RAM.ToString();
                ComputerMotherboardTextBox.Text = selectedComputer.Motherboard;
            }
            else
            {
                EditComputerButton.IsEnabled = false;
                DeleteComputerButton.IsEnabled = false;
            }
        }

        private void AddComputer_Click(object sender, RoutedEventArgs e)
        {
            int computerNumber = int.Parse(ComputerNumberTextBox.Text);
            string status = ComputerStatusTextBox.Text;
            string processor = ComputerProcessorTextBox.Text;
            string graphicsCard = ComputerGraphicsCardTextBox.Text;
            int ram = int.Parse(ComputerRAMTextBox.Text);
            string motherboard = ComputerMotherboardTextBox.Text;

            dbHelper.AddComputer(computerNumber, status, processor, graphicsCard, ram, motherboard);
            MessageBox.Show("Компьютер добавлен");
            LoadComputers();
            ClearComputerFormFields();
        }

        private void EditComputer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedComputer != null)
            {
                int computerNumber = int.Parse(ComputerNumberTextBox.Text);
                string status = ComputerStatusTextBox.Text;
                string processor = ComputerProcessorTextBox.Text;
                string graphicsCard = ComputerGraphicsCardTextBox.Text;
                int ram = int.Parse(ComputerRAMTextBox.Text);
                string motherboard = ComputerMotherboardTextBox.Text;

                dbHelper.UpdateComputer(selectedComputer.ComputerID, computerNumber, status, processor, graphicsCard, ram, motherboard);
                MessageBox.Show("Компьютер обновлен");
                LoadComputers();
                ClearComputerFormFields();
            }
        }

        private void DeleteComputer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedComputer != null)
            {
                dbHelper.DeleteComputer(selectedComputer.ComputerID);
                MessageBox.Show("Компьютер удален");
                LoadComputers();
                ClearComputerFormFields();
            }
        }

        private void ClearComputerFormFields()
        {
            ComputerNumberTextBox.Text = string.Empty;
            ComputerStatusTextBox.Text = string.Empty;
            ComputerProcessorTextBox.Text = string.Empty;
            ComputerGraphicsCardTextBox.Text = string.Empty;
            ComputerRAMTextBox.Text = string.Empty;
            ComputerMotherboardTextBox.Text = string.Empty;
            EditComputerButton.IsEnabled = false;
            DeleteComputerButton.IsEnabled = false;
        }

        private void GamesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GamesDataGrid.SelectedItem != null)
            {
                selectedGame = (Game)GamesDataGrid.SelectedItem;
                EditGameButton.IsEnabled = true;
                DeleteGameButton.IsEnabled = true;

                GameNameTextBox.Text = selectedGame.Name;
                GameDescriptionTextBox.Text = selectedGame.Description;
                GameImageTextBox.Text = selectedGame.ImagePath;
            }
            else
            {
                EditGameButton.IsEnabled = false;
                DeleteGameButton.IsEnabled = false;
            }
        }

        private void AddGame_Click(object sender, RoutedEventArgs e)
        {
            string name = GameNameTextBox.Text;
            string description = GameDescriptionTextBox.Text;
            string imagePath = GameImageTextBox.Text;

            dbHelper.AddGame(name, description, imagePath);
            MessageBox.Show("Игра добавлена");
            LoadGames();
            ClearGameFormFields();
        }

        private void EditGame_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGame != null)
            {
                string name = GameNameTextBox.Text;
                string description = GameDescriptionTextBox.Text;
                string imagePath = GameImageTextBox.Text;

                dbHelper.UpdateGame(selectedGame.GameID, name, description, imagePath);
                MessageBox.Show("Игра обновлена");
                LoadGames();
                ClearGameFormFields();
            }
        }

        private void DeleteGame_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGame != null)
            {
                dbHelper.DeleteGame(selectedGame.GameID);
                MessageBox.Show("Игра удалена");
                LoadGames();
                ClearGameFormFields();
            }
        }

        private void ClearGameFormFields()
        {
            GameNameTextBox.Text = string.Empty;
            GameDescriptionTextBox.Text = string.Empty;
            GameImageTextBox.Text = string.Empty;
            EditGameButton.IsEnabled = false;
            DeleteGameButton.IsEnabled = false;
        }

        private void FoodAndDrinksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FoodAndDrinksDataGrid.SelectedItem != null)
            {
                selectedFoodOrDrink = (FoodOrDrink)FoodAndDrinksDataGrid.SelectedItem;
                EditFoodOrDrinkButton.IsEnabled = true;
                DeleteFoodOrDrinkButton.IsEnabled = true;

                FoodNameTextBox.Text = selectedFoodOrDrink.Name;
                FoodPriceTextBox.Text = selectedFoodOrDrink.Price.ToString();
                FoodQuantityTextBox.Text = selectedFoodOrDrink.Quantity.ToString();
                FoodImageTextBox.Text = selectedFoodOrDrink.ImagePath;
            }
            else
            {
                EditFoodOrDrinkButton.IsEnabled = false;
                DeleteFoodOrDrinkButton.IsEnabled = false;
            }
        }

        private void BookingsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BookingsDataGrid.SelectedItem != null)
            {
                selectedBooking = (Booking)BookingsDataGrid.SelectedItem;
                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }

     

        private void EditFoodOrDrink_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFoodOrDrink != null)
            {
                string name = FoodNameTextBox.Text;
                decimal price = decimal.Parse(FoodPriceTextBox.Text);
                int quantity = int.Parse(FoodQuantityTextBox.Text);
                string imagePath = FoodImageTextBox.Text;

                dbHelper.UpdateFoodOrDrink(selectedFoodOrDrink.Id, name, price, imagePath, quantity);
                MessageBox.Show("Еда или напиток обновлены");
                LoadFoodAndDrinks();
                ClearFoodOrDrinkFormFields();
            }
        }

        private void DeleteFoodOrDrink_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFoodOrDrink != null)
            {
                dbHelper.DeleteFoodOrDrink(selectedFoodOrDrink.Id);
                MessageBox.Show("Еда или напиток удалены");
                LoadFoodAndDrinks();
                ClearFoodOrDrinkFormFields();
            }
        }

        private void ClearFoodOrDrinkFormFields()
        {
            FoodNameTextBox.Text = string.Empty;
            FoodPriceTextBox.Text = string.Empty;
            FoodQuantityTextBox.Text = string.Empty;
            FoodImageTextBox.Text = string.Empty;
            EditFoodOrDrinkButton.IsEnabled = false;
            DeleteFoodOrDrinkButton.IsEnabled = false;
        }

        private void LoadClients()
        {
            List<Client> clients = dbHelper.GetClients();
            ClientComboBox.ItemsSource = clients;
            ClientsDataGrid.ItemsSource = clients;
        }

        private void LoadBookings()
        {
            List<Booking> bookings = dbHelper.GetBookings();
            BookingsDataGrid.ItemsSource = bookings;
        }

        private void LoadComputers()
        {
            List<Computer> computers = dbHelper.GetComputers();
            ComputersDataGrid.ItemsSource = computers;
            ComputerNumberComboBox.ItemsSource = computers;
        }

        private void LoadGames()
        {
            List<Game> games = dbHelper.GetGames();
            GamesDataGrid.ItemsSource = games;
        }

        private void LoadCart()
        {
            var cartItems = dbHelper.GetCartItems(clientId);
            CartDataGrid.ItemsSource = cartItems;
            UpdateTotalCost(cartItems);
        }

        private void LoadFoodAndDrinks()
        {
            List<FoodOrDrink> foodAndDrinks = dbHelper.GetFoodAndDrinks();
            FoodAndDrinksDataGrid.ItemsSource = foodAndDrinks;
        }

        private void UpdateTotalCost(List<CartItem> cartItems)
        {
            decimal totalCost = cartItems.Sum(item => item.ItemPrice * item.Quantity);
            TotalCostTextBlock.Text = $"Итоговая стоимость: {totalCost:C}";
        }

        private decimal CalculateBookingCost(Booking booking)
        {
            TimeSpan duration = booking.EndTime - booking.StartTime;
            decimal hours = (decimal)duration.TotalHours;
            decimal costPerHour = 100m;
            return hours * costPerHour;
        }

        // Добавлено
        private void ShowClients()
        {
            ClientsGroupBox.Visibility = Visibility.Visible;
            ClientsListGroupBox.Visibility = Visibility.Visible;
            ComputersGroupBox.Visibility = Visibility.Collapsed;
            ComputersListGroupBox.Visibility = Visibility.Collapsed;
            GamesGroupBox.Visibility = Visibility.Collapsed;
            GamesListGroupBox.Visibility = Visibility.Collapsed;
            BookingGroupBox.Visibility = Visibility.Collapsed;
            BookingListGroupBox.Visibility = Visibility.Collapsed;
            FoodAndDrinksGroupBox.Visibility = Visibility.Collapsed;
            CartGroupBox.Visibility = Visibility.Collapsed;
        }

        private void ShowComputers()
        {
            ClientsGroupBox.Visibility = Visibility.Collapsed;
            ClientsListGroupBox.Visibility = Visibility.Collapsed;
            ComputersGroupBox.Visibility = Visibility.Visible;
            ComputersListGroupBox.Visibility = Visibility.Visible;
            GamesGroupBox.Visibility = Visibility.Collapsed;
            GamesListGroupBox.Visibility = Visibility.Collapsed;
            BookingGroupBox.Visibility = Visibility.Collapsed;
            BookingListGroupBox.Visibility = Visibility.Collapsed;
            FoodAndDrinksGroupBox.Visibility = Visibility.Collapsed;
            CartGroupBox.Visibility = Visibility.Collapsed;
        }

        private void ShowGames()
        {
            ClientsGroupBox.Visibility = Visibility.Collapsed;
            ClientsListGroupBox.Visibility = Visibility.Collapsed;
            ComputersGroupBox.Visibility = Visibility.Collapsed;
            ComputersListGroupBox.Visibility = Visibility.Collapsed;
            GamesGroupBox.Visibility = Visibility.Visible;
            GamesListGroupBox.Visibility = Visibility.Visible;
            BookingGroupBox.Visibility = Visibility.Collapsed;
            BookingListGroupBox.Visibility = Visibility.Collapsed;
            FoodAndDrinksGroupBox.Visibility = Visibility.Collapsed;
            CartGroupBox.Visibility = Visibility.Collapsed;
        }

        private void ShowBookings()
        {
            ClientsGroupBox.Visibility = Visibility.Collapsed;
            ClientsListGroupBox.Visibility = Visibility.Collapsed;
            ComputersGroupBox.Visibility = Visibility.Collapsed;
            ComputersListGroupBox.Visibility = Visibility.Collapsed;
            GamesGroupBox.Visibility = Visibility.Collapsed;
            GamesListGroupBox.Visibility = Visibility.Collapsed;
            BookingGroupBox.Visibility = Visibility.Visible;
            BookingListGroupBox.Visibility = Visibility.Visible;
            FoodAndDrinksGroupBox.Visibility = Visibility.Collapsed;
            CartGroupBox.Visibility = Visibility.Collapsed;
        }

        private void ShowFoodAndDrinks()
        {
            ClientsGroupBox.Visibility = Visibility.Collapsed;
            ClientsListGroupBox.Visibility = Visibility.Collapsed;
            ComputersGroupBox.Visibility = Visibility.Collapsed;
            ComputersListGroupBox.Visibility = Visibility.Collapsed;
            GamesGroupBox.Visibility = Visibility.Collapsed;
            GamesListGroupBox.Visibility = Visibility.Collapsed;
            BookingGroupBox.Visibility = Visibility.Collapsed;
            BookingListGroupBox.Visibility = Visibility.Collapsed;
            FoodAndDrinksGroupBox.Visibility = Visibility.Visible;
            CartGroupBox.Visibility = Visibility.Collapsed;
        }

        private void ShowCart()
        {
            ClientsGroupBox.Visibility = Visibility.Collapsed;
            ClientsListGroupBox.Visibility = Visibility.Collapsed;
            ComputersGroupBox.Visibility = Visibility.Collapsed;
            ComputersListGroupBox.Visibility = Visibility.Collapsed;
            GamesGroupBox.Visibility = Visibility.Collapsed;
            GamesListGroupBox.Visibility = Visibility.Collapsed;
            BookingGroupBox.Visibility = Visibility.Collapsed;
            BookingListGroupBox.Visibility = Visibility.Collapsed;
            FoodAndDrinksGroupBox.Visibility = Visibility.Collapsed;
            CartGroupBox.Visibility = Visibility.Visible;
        }
    }
}
