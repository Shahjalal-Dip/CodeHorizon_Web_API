using Blazor.Toast.Services;
using CodeHorizon.Blazor.Interfaces;
using CodeHorizon.Blazor.Models.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace CodeHorizon.Blazor.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ToastService _toastService;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ToastService toastService, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _toastService = toastService;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
        {

            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                var result = await HandleResponse<object>(response);
                return new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DELETE request to {Endpoint}", endpoint);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Network error occurred. Please check your connection."
                };
            }
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error in GET request to {Endpoint}", endpoint);

                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Network error occurred. Please check your connection."
                };
            }
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in POST request to {Endpoint}", endpoint);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Network error occurred. Please check your connection."
                };
            }
        }

        public async Task<ApiResponse<T>> PostFormDataAsync<T>(string endpoint, MultipartFormDataContent content)
        {
            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in POST FormData request to {Endpoint}", endpoint);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Network error occurred. Please check your connection."
                };
            }
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PUT request to {Endpoint}", endpoint);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Network error occurred. Please check your connection."
                };
            }
        }
     
        private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if(response.IsSuccessStatusCode)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new ApiResponse<T>
                    {
                        Success = true,
                        Data = data,
                        StatusCode = (int)response.StatusCode
                    };
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "JSON deserialization error");
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = "Invalid response format from server",
                        StatusCode = (int)response.StatusCode
                    };
                }
            }
            else
            {
                try
                {
                    var error = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _toastService.ShowError(error?.Detail ?? "An error occurred");

                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = error?.Detail ?? response.ReasonPhrase ?? "Unknown error",
                        StatusCode = (int)response.StatusCode
                    };
                }
                catch
                {
                    return new ApiResponse<T> { 
                        Success = false,
                        Message = $"HTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}",
                        StatusCode = (int)response.StatusCode
                    };
                }
            }
        }
    }
}
