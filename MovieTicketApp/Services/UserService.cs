using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketApp.Models;

namespace MovieTicketApp.Services
{
    public class UserService
    {
        private readonly ApiClient _apiClient;
        private const string BaseEndpoint = "users";

        public UserService()
        {
            _apiClient = new ApiClient();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _apiClient.getasync<List<User>>(BaseEndpoint);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get all users: {ex.Message}");
            }
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            try
            {
                return await _apiClient.getasync<List<User>>($"{BaseEndpoint}/role/{role}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get users with role {role}: {ex.Message}");
            }
        }

        public async Task<User> RegisterAsync(string username, string password, string role)
        {
            var registerData = new
            {
                username = username,
                password = password,
                role = role
            };

            try
            {
                System.Diagnostics.Debug.WriteLine($"Attempting register for user: {username}");
                var user = await _apiClient.postasync<User>("auth/register", registerData);
                System.Diagnostics.Debug.WriteLine($"Register successful for user: {user?.Username}");
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var loginData = new
            {
                username = username,
                password = password
            };

            try
            {
                //return await _apiClient.postasync<User>($"auth/login", loginData);
                System.Diagnostics.Debug.WriteLine($"Attempting login for user: {username}");
                var user = await _apiClient.postasync<User>("auth/login", loginData);
                System.Diagnostics.Debug.WriteLine($"Login successful for user: {user?.Username}");
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task<User> GetUserProfileAsync(int userId)
        {
            try
            {
                return await _apiClient.getasync<User>($"{BaseEndpoint}/{userId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get user profile: {ex.Message}");
            }
        }

        public async Task<User> UpdateUserProfileAsync(int userId, User userData)
        {
            try
            {
                var userDataSend = new
                {
                    username = userData.Username,
                    password = userData.Password,
                    role = userData.Role,
                };
                return await _apiClient.putasync<User>($"{BaseEndpoint}/{userId}", userDataSend);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update user profile: {ex.Message}");
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                await _apiClient.deleteasync($"{BaseEndpoint}/{userId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete staff: {ex.Message}");
            }
        }

        public async Task<User> UpdatePreferencesAsync(int userId, string preferences)
        {
            try
            {
                var preferencesData = new
                {
                    preferences = preferences
                };

                var response = await _apiClient.putasync<User>($"{BaseEndpoint}/{userId}/preferences", preferencesData);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update preferences: {ex.Message}");
            }
        }
    }
}
