using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketApp.Models
{
    public class PurchaseHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public string PurchaseDate { get; set; } // Use string for simplicity
    }
}
