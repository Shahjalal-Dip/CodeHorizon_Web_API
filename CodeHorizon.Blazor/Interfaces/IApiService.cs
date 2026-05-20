using CodeHorizon.Blazor.Models.Common;

namespace CodeHorizon.Blazor.Interfaces
{
    public interface IApiService
    {
        Task<ApiResponse<T>> GetAsync<T>(string endpoint);
        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data);
        Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data);
        Task<ApiResponse<bool>> DeleteAsync(string endpoint);
        Task<ApiResponse<T>> PostFormDataAsync<T>(string endpoint, MultipartFormDataContent content);

    }
}
