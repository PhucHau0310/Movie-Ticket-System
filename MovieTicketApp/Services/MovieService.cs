using MovieTicketApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketApp.Services
{
    public class MovieService
    {
        private readonly ApiClient _apiClient;
        private const string BaseEndpoint = "movies";

        public MovieService()
        {
            _apiClient = new ApiClient();
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _apiClient.getasync<List<Movie>>(BaseEndpoint);
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            var movieDataSend = new
            {
                title = movie.Title,
                description = movie.Description,
                genre = movie.Genre,
                duration = movie.Duration,
                release_date = movie.ReleaseDate,
                price = movie.Price,
                rating = movie.Rating,
                image_url = movie.ImageUrl
            };
            return await _apiClient.postasync<Movie>(BaseEndpoint, movieDataSend);
        }

        public async Task<Movie> UpdateMovieAsync(int id, Movie movie)
        {
            return await _apiClient.putasync<Movie>($"{BaseEndpoint}/{id}", movie);
        }

        public async Task DeleteMovieAsync(int id)
        {
            await _apiClient.deleteasync($"{BaseEndpoint}/{id}");
        }

        public async Task<List<Movie>> GetRecommendationsAsync(int userId)
        {
            try
            {
                return await _apiClient.getasync<List<Movie>>($"recommendations/{userId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get recommendations: {ex.Message}");
            }
        }
    }
}
