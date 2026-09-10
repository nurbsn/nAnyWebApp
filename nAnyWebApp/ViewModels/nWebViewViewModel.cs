using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using nAnyWebApp.Models;
using nAnyWebApp.Services;

namespace nAnyWebApp.ViewModels;

[QueryProperty(nameof(WebApp), "WebApp")]
[QueryProperty(nameof(WebAppId), "WebAppId")]
public partial class nWebViewViewModel : BaseViewModel
{
    private readonly IScriptInjectorService _scriptInjectorService;
    private readonly IShortcutService _shortcutService;
    private readonly IWebAppService _webAppService;

    [ObservableProperty]
    private WebApp? webApp;

    [ObservableProperty]
    private string webAppId = string.Empty;

    [ObservableProperty]
    private string source = "https://duckduckgo.com";

    [ObservableProperty]
    private string pageTitle = "Ładowanie strony...";

    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty]
    private bool isToolbarVisible = true;

    [ObservableProperty]
    private bool isCssActive;

    [ObservableProperty]
    private bool isJsActive;

    public nWebViewViewModel(
        IScriptInjectorService scriptInjectorService,
        IShortcutService shortcutService,
        IWebAppService webAppService)
    {
        _scriptInjectorService = scriptInjectorService;
        _shortcutService = shortcutService;
        _webAppService = webAppService;
    }

    async partial void OnWebAppIdChanged(string value)
    {
        if (Guid.TryParse(value, out var id))
        {
            var found = await _webAppService.GetByIdAsync(id);
            if (found != null)
            {
                WebApp = found;
            }
        }
    }

    partial void OnWebAppChanged(WebApp? value)
    {
        if (value != null)
        {
            Title = value.Name;
            PageTitle = value.Name;
            Source = value.Url;
            IsCssActive = value.HasCustomCss;
            IsJsActive = value.HasCustomJs;
            IsLoading = true;
        }
    }

    [RelayCommand]
    public async Task WebViewNavigated(object? parameter)
    {
        IsLoading = false;

        if (parameter is Tuple<WebNavigatedEventArgs, WebView> tuple)
        {
            var e = tuple.Item1;
            var webView = tuple.Item2;

            if (e.Result == WebNavigationResult.Success)
            {
                PageTitle = WebApp?.Name ?? e.Url;

                // Wstrzykiwanie skryptów CSS i JS po załadowaniu DOM (globalnych oraz podstron)
                if (WebApp != null && webView != null)
                {
                    await _scriptInjectorService.InjectAllAsync(webView, WebApp, e.Url);
                }
            }
            else
            {
                PageTitle = "Błąd ładowania";
            }
        }
    }

    [RelayCommand]
    private void NavigateBack(WebView webView)
    {
        if (webView != null && webView.CanGoBack)
        {
            webView.GoBack();
        }
    }

    [RelayCommand]
    private void NavigateForward(WebView webView)
    {
        if (webView != null && webView.CanGoForward)
        {
            webView.GoForward();
        }
    }

    [RelayCommand]
    private void RefreshPage(WebView webView)
    {
        if (webView != null)
        {
            IsLoading = true;
            webView.Reload();
        }
    }

    [RelayCommand]
    private async Task ReinjectScripts(WebView webView)
    {
        if (WebApp != null && webView != null)
        {
            bool success = await _scriptInjectorService.InjectAllAsync(webView, WebApp);
            if (success)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Skrypty", "Pomyślnie wstrzyknięto niestandardowy CSS i JavaScript!", "OK");
                }
            }
            else
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Informacja", "Wstrzykiwanie zakończone lub strona nie posiada aktywnych skryptów.", "OK");
                }
            }
        }
    }

    [RelayCommand]
    private void ToggleFullscreen()
    {
        IsToolbarVisible = !IsToolbarVisible;
    }

    [RelayCommand]
    private async Task CreateShortcut()
    {
        if (WebApp == null) return;

        if (!_shortcutService.IsSupported)
        {
            await Shell.Current.DisplayAlert("Informacja", "Tworzenie skrótów nie jest obsługiwane na tej platformie.", "OK");
            return;
        }

        bool success = await _shortcutService.CreateShortcutAsync(WebApp);
        if (success)
        {
#if WINDOWS
            await Shell.Current.DisplayAlert("Sukces", $"Utworzono skrót do „{WebApp.Name}” na Twoim Pulpicie!", "OK");
#elif ANDROID
            await Shell.Current.DisplayAlert("Sukces", $"Wysłano prośbę o dodanie skrótu „{WebApp.Name}” do ekranu głównego.", "OK");
#else
            await Shell.Current.DisplayAlert("Sukces", "Skrót został utworzony.", "OK");
#endif
        }
        else
        {
            await Shell.Current.DisplayAlert("Błąd", "Nie udało się utworzyć skrótu.", "OK");
        }
    }

    [RelayCommand]
    private void ToggleMenu()
    {
        if (Shell.Current != null)
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
        }
    }

    [RelayCommand]
    private async Task OpenInBrowser()
    {
        if (!string.IsNullOrWhiteSpace(Source))
        {
            await Launcher.OpenAsync(Source);
        }
    }

    [RelayCommand]
    private async Task Close()
    {
        try
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas powrotu do MainPage: {ex.Message}");
        }
    }
}
