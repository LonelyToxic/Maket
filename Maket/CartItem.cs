using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maket.Models
{
    public class CartItem
    {
        public int CartID { get; set; }
        public int ClientID { get; set; }
        public int ComputerNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }
        public string ItemName { get; set; }  // Добавлено
        public decimal ItemPrice { get; set; }  // Добавлено
    }
}


