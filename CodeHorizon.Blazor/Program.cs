using System.Net;
using CodeHorizon.Blazor;
using CodeHorizon.Blazor.Helpers.Validators;
using CodeHorizon.Blazor.Models.Config;
using CodeHorizon.Blazor.Providers;
using CodeHorizon.Blazor.Services;
using CodeHorizon.Blazor.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Polly;
using Polly.Extensions.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));
builder.Services.Configure<FeatureFlags>(builder.Configuration.GetSection(FeatureFlags.SectionName));

var apiSettings = builder.Configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>() ?? new ApiSettings();

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiSettings.ApiV1Base.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(apiSettings.RequestTimeoutSeconds);
})
.AddHttpMessageHandler<AuthorizationMessageHandler>()
.AddPolicyHandler(GetRetryPolicy(apiSettings.RetryCount));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISnippetService, SnippetService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddSingleton<StateContainer>();
builder.Services.AddSingleton<IToastService, ToastService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IConnectivityService, ConnectivityService>();
builder.Services.AddScoped<ClipboardInterop>();
builder.Services.AddScoped<PrismInterop>();

var host = builder.Build();

var theme = host.Services.GetRequiredService<IThemeService>();
await theme.InitializeAsync();

var connectivity = host.Services.GetRequiredService<IConnectivityService>();
await connectivity.InitializeAsync();

await host.RunAsync();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount) =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(retryCount, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
