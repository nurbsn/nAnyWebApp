using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using nAnyWebApp.Models;
using nAnyWebApp.Services;

namespace nAnyWebApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IWebAppService _webAppService;
    private readonly IShortcutService _shortcutService;
    private List<WebApp> _allWebApps = new();

    [ObservableProperty]
    private ObservableCollection<WebApp> webApps = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isEmpty;

    public MainViewModel(IWebAppService webAppService, IShortcutService shortcutService)
    {
        _webAppService = webAppService;
        _shortcutService = shortcutService;
        Title = "Moje Aplikacje";
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        await LoadWebAppsAsync();
    }

    [RelayCommand]
    public async Task LoadWebAppsAsync()
    {
        IsRefreshing = true;
        try
        {
            _allWebApps = await _webAppService.GetAllAsync();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas ładowania aplikacji: {ex.Message}");
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się załadować listy stron.", "OK");
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            WebApps = new ObservableCollection<WebApp>(_allWebApps);
        }
        else
        {
            var query = SearchText.Trim();
            var filtered = _allWebApps.Where(a =>
                (a.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (a.Url?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (a.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
            );
            WebApps = new ObservableCollection<WebApp>(filtered);
        }

        IsEmpty = WebApps.Count == 0;
    }

    [RelayCommand]
    private async Task OpenWebApp(WebApp app)
    {
        if (app == null) return;

        await _webAppService.RecordOpenedAsync(app.Id);

        try
        {
            await Shell.Current.GoToAsync($"//webapp_{app.Id:N}");
        }
        catch
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "WebApp", app }
            };
            await Shell.Current.GoToAsync(nameof(nWebViewPage), navigationParameter);
        }
    }

    [RelayCommand]
    private async Task AddNewWebApp()
    {
        await Shell.Current.GoToAsync(nameof(WebAppEditPage));
    }

    [RelayCommand]
    private async Task EditWebApp(WebApp app)
    {
        if (app == null) return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "WebApp", app }
        };

        await Shell.Current.GoToAsync(nameof(WebAppEditPage), navigationParameter);
    }

    [RelayCommand]
    private async Task DeleteWebApp(WebApp app)
    {
        if (app == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Usuń stronę",
            $"Czy na pewno chcesz usunąć aplikację „{app.Name}”?",
            "Usuń",
            "Anuluj"
        );

        if (confirm)
        {
            await _webAppService.DeleteAsync(app.Id);
            await LoadWebAppsAsync();
        }
    }

    [RelayCommand]
    private async Task CreateShortcut(WebApp app)
    {
        if (app == null) return;

        if (!_shortcutService.IsSupported)
        {
            await Shell.Current.DisplayAlert("Informacja", "Tworzenie skrótów nie jest obsługiwane na tej platformie.", "OK");
            return;
        }

        bool success = await _shortcutService.CreateShortcutAsync(app);
        if (success)
        {
#if WINDOWS
            await Shell.Current.DisplayAlert("Sukces", $"Utworzono skrót do „{app.Name}” na Twoim Pulpicie!", "OK");
#elif ANDROID
            await Shell.Current.DisplayAlert("Sukces", $"Wysłano prośbę o dodanie skrótu „{app.Name}” do ekranu głównego.", "OK");
#else
            await Shell.Current.DisplayAlert("Sukces", "Skrót został utworzony.", "OK");
#endif
        }
        else
        {
            await Shell.Current.DisplayAlert("Błąd", "Nie udało się utworzyć skrótu na ekranie głównym.", "OK");
        }
    }
}
