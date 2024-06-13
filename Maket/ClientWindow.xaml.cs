using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Maket.Models;

namespace Maket
{
    public partial class ClientWindow : Window
    {
        private int clientId;
        private CartManager cartManager;
        private DatabaseHelper dbHelper;

        public ClientWindow(int clientId)
        {
            InitializeComponent();
            this.clientId = clientId;
            cartManager = new CartManager();
            dbHelper = new DatabaseHelper();
            LoadComputers();
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFoodOrDrink = (FoodOrDrink)FoodAndDrinksDataGrid.SelectedItem;
            if (selectedFoodOrDrink != null)
            {
                cartManager.AddToCart(new CartItem
                {
                    ClientID = clientId,
                    ItemName = selectedFoodOrDrink.Name,
                    ItemPrice = selectedFoodOrDrink.Price,
                    Quantity = 1
                });
                MessageBox.Show($"{selectedFoodOrDrink.Name} добавлен в корзину.");
                LoadCart();
            }
        }

        private void LoadComputers()
        {
            var computers = dbHelper.GetComputers();
            ComputerComboBox.ItemsSource = computers;
        }

        private void ShowBookings_Click(object sender, RoutedEventArgs e)
        {
            BookingsGroupBox.Visibility = Visibility.Visible;
            GamesGroupBox.Visibility = Visibility.Collapsed;
            FoodAndDrinksGroupBox.Visibility = Visibility.Collapsed;
            CartGroupBox.Visibility = Visibility.Collapsed;
            AddBookingGroupBox.Visibility = Visibility.Visible;

            LoadBookings();
        }

        private void ShowGames_Click(object sender, RoutedEventArgs e)
        {
            BookingsGroupBox.Visibility = Visibility.Collapsed;
            GamesGroupBox.Visibility = Visibility.Visible;
            FoodAndDrinksGroupBox.Visibility = Visibility.Collapsed;
            CartGroupBox.Visibility = Visibility.Collapsed;
            AddBookingGroupBox.Visibility = Visibility.Collapsed;

            LoadGames();
        }

        private void ShowFoodAndDrinks_Click(object sender, RoutedEventArgs e)
        {
            BookingsGroupBox.Visibility = Visibility.Collapsed;
            GamesGroupBox.Visibility = Visibility.Collapsed;
            FoodAndDrinksGroupBox.Visibility = Visibility.Visible;
            CartGroupBox.Visibility = Visibility.Collapsed;
            AddBookingGroupBox.Visibility = Visibility.Collapsed;

            LoadFoodAndDrinks();
        }

        private void ShowCart_Click(object sender, RoutedEventArgs e)
        {
            BookingsGroupBox.Visibility = Visibility.Collapsed;
            GamesGroupBox.Visibility = Visibility.Collapsed;
            FoodAndDrinksGroupBox.Visibility = Visibility.Collapsed;
            CartGroupBox.Visibility = Visibility.Visible;
            AddBookingGroupBox.Visibility = Visibility.Collapsed;

            LoadCart();
        }

        private void LoadBookings()
        {
            var bookings = dbHelper.GetBookingsByClientId(clientId);
            BookingsDataGrid.ItemsSource = bookings;
        }

        private void LoadGames()
        {
            var games = dbHelper.GetGames();
            GamesDataGrid.ItemsSource = games;
        }

        private void LoadFoodAndDrinks()
        {
            var foodAndDrinks = dbHelper.GetFoodAndDrinks();
            FoodAndDrinksDataGrid.ItemsSource = foodAndDrinks;
        }

        private void LoadCart()
        {
            var cartItems = cartManager.GetCartItemsByClientId(clientId);
            CartDataGrid.ItemsSource = cartItems;
            UpdateTotalCost(cartItems);
        }

        private void AddFoodOrDrinkToCart_Click(object sender, RoutedEventArgs e)
        {
            var selectedFoodOrDrink = (FoodOrDrink)FoodAndDrinksDataGrid.SelectedItem;
            if (selectedFoodOrDrink != null)
            {
                cartManager.AddFoodOrDrinkToCart(new CartItem
                {
                    ClientID = clientId,
                    ItemName = selectedFoodOrDrink.Name,
                    ItemPrice = selectedFoodOrDrink.Price,
                    Quantity = 1
                });
                MessageBox.Show("Еда или напиток добавлены в корзину");
                LoadCart();
            }
        }

        private void ExtendBooking_Click(object sender, RoutedEventArgs e)
        {
            if (BookingsDataGrid.SelectedItem is Booking selectedBooking)
            {
                var newEndTime = selectedBooking.EndTime.Add(new TimeSpan(1, 0, 0)); // Добавляем 1 час
                dbHelper.UpdateBooking(selectedBooking.BookingID, selectedBooking.ClientID ?? 0, selectedBooking.ComputerNumber, selectedBooking.BookingDate, selectedBooking.StartTime, newEndTime);
                MessageBox.Show("Бронирование продлено");
                LoadBookings();
            }
        }

        private void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            if (BookingsDataGrid.SelectedItem is Booking selectedBooking)
            {
                dbHelper.DeleteBooking(selectedBooking.BookingID);
                MessageBox.Show("Бронирование отменено");
                LoadBookings();
            }
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            cartManager.CompletePurchase(clientId);
            MessageBox.Show("Оплата прошла успешно");
            LoadCart();
        }

        private void UpdateTotalCost(List<CartItem> cartItems)
        {
            decimal totalCost = cartItems.Sum(item => item.ItemPrice * item.Quantity);
            TotalCostTextBlock.Text = $"Итоговая стоимость: {totalCost:C}";
        }

        private void AddBooking_Click(object sender, RoutedEventArgs e)
        {
            if (ComputerComboBox.SelectedItem == null || DatePicker.SelectedDate == null || StartTimePicker.Value == null || EndTimePicker.Value == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            int computerId = (int)ComputerComboBox.SelectedValue;
            DateTime date = DatePicker.SelectedDate.Value;
            TimeSpan startTime = StartTimePicker.Value.Value.TimeOfDay;
            TimeSpan endTime = EndTimePicker.Value.Value.TimeOfDay;

            var booking = new Booking
            {
                ClientID = clientId,
                ComputerNumber = computerId,
                BookingDate = date,
                StartTime = startTime,
                EndTime = endTime,
                Cost = CalculateBookingCost(startTime, endTime)
            };

            dbHelper.AddBooking(booking);
            MessageBox.Show("Бронирование добавлено");
            LoadBookings();
        }

        private decimal CalculateBookingCost(TimeSpan startTime, TimeSpan endTime)
        {
            TimeSpan duration = endTime - startTime;
            decimal hours = (decimal)duration.TotalHours;
            decimal costPerHour = 100m;
            return hours * costPerHour;
        }
    }
}
