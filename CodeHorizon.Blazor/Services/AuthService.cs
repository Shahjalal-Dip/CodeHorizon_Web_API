using Blazor.Toast.Services;
using CodeHorizon.Blazor.Interfaces;
using CodeHorizon.Blazor.Models.Auth;
using CodeHorizon.Blazor.Providers;
using Markdig.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace CodeHorizon.Blazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ToastService _toastService;

        private const string TokenKey = "authToken";
        private const string UserKey = "currentUser";

        public AuthService(IApiService apiService,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider,
        ToastService toastService)
        {
            _apiService = apiService;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _toastService = toastService;
        }

        public async Task<AuthResponse?> Login(LoginRequest request)
        {
            var response = await _apiService.PostAsync<AuthResponse>("/auth/login", request);

            if(response.Success && response.Data != null) 
            {
                await _localStorage.SetItemAsync(TokenKey, response.Data.Token);
                await _localStorage.SetItemAsync(UserKey, response.Data);

                ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(response.Data.Token);

                _toastService.ShowSuccess($"Welcome back, {response.Data.FullName}!");
                return response.Data;
            }

            return null;
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            var response = await _apiService.PostAsync<object>("/auth/register", request);

            if (response.Success)
            {
                _toastService.ShowSuccess("Registration successful! Please login.");
                return true;
            }

            return false;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            await _localStorage.RemoveItemAsync(UserKey);
            ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
            _toastService.ShowInfo("You have been logged out.");
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await GetToken();
            return  !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetToken()
        {
            return await _localStorage.GetItemAsync<string>(TokenKey);
        }

        public async Task<AuthResponse?> GetCurrentUser()
        {
            return await _localStorage.GetItemAsync<AuthResponse>(UserKey);
        }
    }
}
