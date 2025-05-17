using MovieTicketApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketApp.Services
{
    public class TicketService
    {
        private readonly ApiClient _apiClient;
        private const string BaseEndpoint = "tickets";

        public TicketService()
        {
            _apiClient = new ApiClient();
        }

        public async Task<List<TicketResponse>> GetAllTicketsAsync()
        {
            try
            {
                return await _apiClient.getasync<List<TicketResponse>>(BaseEndpoint);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get all tickets: {ex.Message}");
            }
        }

        public async Task<PaymentResponse> CreateTicketAsync(TicketCreate ticket)
        {
            return await _apiClient.postasync<PaymentResponse>(BaseEndpoint, ticket);
        }

        public async Task<List<TicketResponse>> GetTicketsByUserAsync(int userId)
        {
            try
            {
                return await _apiClient.getasync<List<TicketResponse>>($"{BaseEndpoint}/user/{userId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get tickets: {ex.Message}");
            }
        }
    }

    public class TicketResponse
    {
        public int id { get; set; }
        public string seat_numbers { get; set; } // Comma-separated seat numbers
        public int user_id { get; set; }
        public int movie_id { get; set; }
        public float total_price { get; set; }
        public string purchase_date { get; set; } // Use string for simplicity
        public string status { get; set; } = "Success"; // Default status
    }

    public class TicketCreate
    {
        public string seat_numbers { get; set; }
        public int user_id { get; set; }
        public int movie_id { get; set; }
    }

    public class PaymentResponse
    {
        public string payment_url { get; set; }
        public int ticket_id { get; set; }
    }
}
