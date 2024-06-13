using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using Maket.Models;
using Microsoft.Win32;

namespace Maket
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            connectionString = "Data Source=DESKTOP-A62UMV9;Initial Catalog=master;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        }

        // Метод для проверки пользователя
        public ValidationResult ValidateUser(string username, string password)
        {
            ValidationResult result = new ValidationResult();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT UserType, UserID FROM UserAccounts WHERE Username = @Username AND Password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result.IsValid = true;
                        result.UserType = reader["UserType"].ToString();
                        result.UserId = reader["UserID"] != DBNull.Value ? (int?)reader["UserID"] : null;
                    }
                    else
                    {
                        result.IsValid = false;
                    }
                }
            }
            return result;
        }

        public void RegisterUser(string username, string password, string userType)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO UserAccounts (Username, Password, UserType) VALUES (@Username, @Password, @UserType)", conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@UserType", userType);
                cmd.ExecuteNonQuery();
            }
        }

        public List<Client> GetClients()
        {
            List<Client> clients = new List<Client>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clients", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clients.Add(new Client
                    {
                        ClientID = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Age = reader.GetInt32(2),
                        ContactInfo = reader.GetString(3)
                    });
                }
            }
            return clients;
        }

        public List<Computer> GetComputers()
        {
            List<Computer> computers = new List<Computer>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Computers", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    computers.Add(new Computer
                    {
                        ComputerID = reader.GetInt32(0),
                        ComputerNumber = reader.GetInt32(1),
                        Status = reader.GetString(2),
                        Processor = reader.GetString(3),
                        GraphicsCard = reader.GetString(4),
                        RAM = reader.GetInt32(5),
                        Motherboard = reader.GetString(6)
                    });
                }
            }
            return computers;
        }

        public List<Booking> GetBookings()
        {
            List<Booking> bookings = new List<Booking>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT BookingID, ComputerNumber, BookingDate, StartTime, EndTime, ClientID, Cost FROM Booking";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Booking booking = new Booking
                        {
                            BookingID = reader.GetInt32(reader.GetOrdinal("BookingID")),
                            ComputerNumber = reader.GetInt32(reader.GetOrdinal("ComputerNumber")),
                            BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                            StartTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                            EndTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime")),
                            ClientID = reader.IsDBNull(reader.GetOrdinal("ClientID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ClientID")),
                            Cost = reader.IsDBNull(reader.GetOrdinal("Cost")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Cost"))
                        };
                        bookings.Add(booking);
                    }
                }
            }
            return bookings;
        }

        public List<Game> GetGames()
        {
            List<Game> games = new List<Game>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Games", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    games.Add(new Game
                    {
                        GameID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        ImagePath = reader.GetString(3)
                    });
                }
            }
            return games;
        }

        public List<FoodOrDrink> GetFoodAndDrinks()
        {
            List<FoodOrDrink> foodAndDrinks = new List<FoodOrDrink>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM FoodAndDrinks", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    foodAndDrinks.Add(new FoodOrDrink
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        ImagePath = reader.GetString(3),
                        Quantity = reader.GetInt32(4)
                    });
                }
            }
            return foodAndDrinks;
        }

        public void UpdateFoodOrDrink(int id, string name, decimal price, string imagePath, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE FoodAndDrinks SET Name = @Name, Price = @Price, ImagePath = @ImagePath, Quantity = @Quantity WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddBooking(Booking booking)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Booking (ClientID, ComputerNumber, BookingDate, StartTime, EndTime, Cost) VALUES (@ClientID, @ComputerNumber, @BookingDate, @StartTime, @EndTime, @Cost)", conn);
                cmd.Parameters.AddWithValue("@ClientID", booking.ClientID);
                cmd.Parameters.AddWithValue("@ComputerNumber", booking.ComputerNumber);
                cmd.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                cmd.Parameters.AddWithValue("@StartTime", booking.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", booking.EndTime);
                cmd.Parameters.AddWithValue("@Cost", booking.Cost);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddFoodOrDrink(string name, decimal price, string imagePath, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO FoodAndDrinks (Name, Price, ImagePath, Quantity) VALUES (@Name, @Price, @ImagePath, @Quantity)", conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteFoodOrDrink(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM FoodAndDrinks WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateBooking(int bookingId, int clientId, int computerNumber, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Bookings SET ClientID = @ClientID, ComputerNumber = @ComputerNumber, BookingDate = @BookingDate, StartTime = @StartTime, EndTime = @EndTime WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingId);
                cmd.Parameters.AddWithValue("@ClientID", clientId);
                cmd.Parameters.AddWithValue("@ComputerNumber", computerNumber);
                cmd.Parameters.AddWithValue("@BookingDate", bookingDate);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteBooking(int bookingId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Bookings WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<CartItem> GetCartItems(int clientId)  // Исправлено
        {
            List<CartItem> cartItems = new List<CartItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Cart WHERE ClientID = @ClientID", conn);
                cmd.Parameters.AddWithValue("@ClientID", clientId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cartItems.Add(new CartItem
                    {
                        ClientID = reader.GetInt32(0),
                        ItemName = reader.GetString(1),
                        ItemPrice = reader.GetDecimal(2),
                        Quantity = reader.GetInt32(3)
                    });
                }
            }
            return cartItems;
        }

        public void AddToCart(Booking booking)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Cart (ClientID, ItemName, ItemPrice, Quantity) VALUES (@ClientID, @ItemName, @ItemPrice, @Quantity)", conn);
                cmd.Parameters.AddWithValue("@ClientID", booking.ClientID);
                cmd.Parameters.AddWithValue("@ItemName", "Booking");
                cmd.Parameters.AddWithValue("@ItemPrice", booking.Cost);
                cmd.ExecuteNonQuery();
            }
        }

        // Обновленный метод CompletePurchase, который принимает List<CartItem>
        public void CompletePurchase(List<CartItem> cartItems)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in cartItems)
                        {
                            string query = "INSERT INTO Orders (ClientID, ItemName, Quantity, ItemPrice) VALUES (@ClientID, @ItemName, @Quantity, @ItemPrice)";
                            SqlCommand command = new SqlCommand(query, connection, transaction);
                            command.Parameters.AddWithValue("@ClientID", item.ClientID);
                            command.Parameters.AddWithValue("@ItemName", item.ItemName);
                            command.Parameters.AddWithValue("@Quantity", item.Quantity);
                            command.Parameters.AddWithValue("@ItemPrice", item.ItemPrice);
                            command.ExecuteNonQuery();
                        }

                        string deleteQuery = "DELETE FROM Cart WHERE ClientID = @ClientID";
                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection, transaction);
                        deleteCommand.Parameters.AddWithValue("@ClientID", cartItems.First().ClientID);
                        deleteCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void AddClient(string fullName, int age, string contactInfo)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Clients (FullName, Age, ContactInfo) VALUES (@FullName, @Age, @ContactInfo)", conn);
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@Age", age);
                cmd.Parameters.AddWithValue("@ContactInfo", contactInfo);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateClient(int clientId, string fullName, int age, string contactInfo)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Clients SET FullName = @FullName, Age = @Age, ContactInfo = @ContactInfo WHERE ClientID = @ClientID", conn);
                cmd.Parameters.AddWithValue("@ClientID", clientId);
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@Age", age);
                cmd.Parameters.AddWithValue("@ContactInfo", contactInfo);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteClient(int clientId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Clients WHERE ClientID = @ClientID", conn);
                cmd.Parameters.AddWithValue("@ClientID", clientId);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddComputer(int computerNumber, string status, string processor, string graphicsCard, int ram, string motherboard)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Computers (ComputerNumber, Status, Processor, GraphicsCard, RAM, Motherboard) VALUES (@ComputerNumber, @Status, @Processor, @GraphicsCard, @RAM, @Motherboard)", conn);
                cmd.Parameters.AddWithValue("@ComputerNumber", computerNumber);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Processor", processor);
                cmd.Parameters.AddWithValue("@GraphicsCard", graphicsCard);
                cmd.Parameters.AddWithValue("@RAM", ram);
                cmd.Parameters.AddWithValue("@Motherboard", motherboard);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateComputer(int computerId, int computerNumber, string status, string processor, string graphicsCard, int ram, string motherboard)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Computers SET ComputerNumber = @ComputerNumber, Status = @Status, Processor = @Processor, GraphicsCard = @GraphicsCard, RAM = @RAM, Motherboard = @Motherboard WHERE ComputerID = @ComputerID", conn);
                cmd.Parameters.AddWithValue("@ComputerID", computerId);
                cmd.Parameters.AddWithValue("@ComputerNumber", computerNumber);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Processor", processor);
                cmd.Parameters.AddWithValue("@GraphicsCard", graphicsCard);
                cmd.Parameters.AddWithValue("@RAM", ram);
                cmd.Parameters.AddWithValue("@Motherboard", motherboard);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteComputer(int computerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Computers WHERE ComputerID = @ComputerID", conn);
                cmd.Parameters.AddWithValue("@ComputerID", computerId);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddGame(string name, string description, string imagePath)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Games (Name, Description, ImagePath) VALUES (@Name, @Description, @ImagePath)", conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                cmd.ExecuteNonQuery();
            }
        }


        public void UpdateGame(int gameId, string name, string description, string imagePath)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Games SET Name = @Name, Description = @Description, ImagePath = @ImagePath WHERE GameID = @GameID", conn);
                cmd.Parameters.AddWithValue("@GameID", gameId);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteGame(int gameId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Games WHERE GameID = @GameID", conn);
                cmd.Parameters.AddWithValue("@GameID", gameId);
                cmd.ExecuteNonQuery();
            }
        }

        // Добавлено
        public List<Booking> GetBookingsByClientId(int clientId)
        {
            List<Booking> bookings = new List<Booking>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Bookings WHERE ClientID = @ClientID", conn);
                cmd.Parameters.AddWithValue("@ClientID", clientId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookings.Add(new Booking
                    {
                        BookingID = reader.GetInt32(0),
                        ClientID = reader.GetInt32(1),
                        ComputerNumber = reader.GetInt32(2),
                        BookingDate = reader.GetDateTime(3),
                        StartTime = reader.GetTimeSpan(4),
                        EndTime = reader.GetTimeSpan(5),
                        Cost = reader.GetDecimal(6)
                    });
                }
            }
            return bookings;
        }
    }
}
