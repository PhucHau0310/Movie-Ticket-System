using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string SeatNumbers { get; set; } // Comma-separated seat numbers
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public float TotalPrice { get; set; }
        public string PurchaseDate { get; set; } // Use string for simplicity
        public string Status { get; set; } = "pending"; // Default status is "pending"
    }
}
