using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using nAnyWebApp.Services;

namespace nAnyWebApp.ViewModels;

public partial class ustawieniaViewModel : BaseViewModel
{
    private readonly IWebAppService _webAppService;

    [ObservableProperty]
    private string appVersion = "1.0.0";

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public ustawieniaViewModel(IWebAppService webAppService)
    {
        _webAppService = webAppService;
        Title = "Ustawienia";
        AppVersion = AppInfo.Current.VersionString;
    }

    [RelayCommand]
    private async Task ExportData()
    {
        try
        {
            var json = await _webAppService.ExportToJsonAsync();
            await Clipboard.Default.SetTextAsync(json);
            await Shell.Current.DisplayAlert("Kopia zapasowa", "Konfiguracja Twoich aplikacji została skopiowana do schowka w formacie JSON!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się wyeksportować danych: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ImportData()
    {
        try
        {
            string json = await Shell.Current.DisplayPromptAsync(
                "Importuj strony",
                "Wklej konfigurację w formacie JSON:",
                "Importuj",
                "Anuluj",
                keyboard: Keyboard.Chat
            );

            if (string.IsNullOrWhiteSpace(json)) return;

            bool success = await _webAppService.ImportFromJsonAsync(json);
            if (success)
            {
                await Shell.Current.DisplayAlert("Sukces", "Pomyślnie zaimportowano strony internetowe!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Błąd", "Podany tekst nie jest poprawnym plikiem JSON.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Błąd podczas importu: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task OpenGitHub()
    {
        await Launcher.OpenAsync("https://github.com/nurbsn/nAnyWebApp");
    }
}
