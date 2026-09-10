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

[QueryProperty(nameof(WebApp), "WebApp")]
public partial class WebAppEditViewModel : BaseViewModel
{
    private readonly IWebAppService _webAppService;

    [ObservableProperty]
    private WebApp? webApp;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string url = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string iconUrl = string.Empty;

    [ObservableProperty]
    private string customCss = string.Empty;

    [ObservableProperty]
    private string customJs = string.Empty;

    [ObservableProperty]
    private bool isCustomCssEnabled = true;

    [ObservableProperty]
    private bool isCustomJsEnabled = true;

    [ObservableProperty]
    private string userAgent = string.Empty;

    // Reguły podstron (URL Fragment / Pattern)
    [ObservableProperty]
    private ObservableCollection<UrlScriptRule> urlRules = new();

    [ObservableProperty]
    private bool isAddingUrlRule;

    [ObservableProperty]
    private string newRuleName = string.Empty;

    [ObservableProperty]
    private string newRuleUrlPattern = string.Empty;

    [ObservableProperty]
    private string newRuleCustomCss = string.Empty;

    [ObservableProperty]
    private string newRuleCustomJs = string.Empty;

    public ScriptPreset[] CssPresets => ScriptPresets.CssPresets;
    public ScriptPreset[] JsPresets => ScriptPresets.JsPresets;

    public WebAppEditViewModel(IWebAppService webAppService)
    {
        _webAppService = webAppService;
        Title = "Nowa strona";
    }

    partial void OnWebAppChanged(WebApp? value)
    {
        if (value != null)
        {
            IsEditing = true;
            Title = $"Edycja: {value.Name}";
            Name = value.Name;
            Url = value.Url;
            Description = value.Description;
            IconUrl = value.IconUrl;
            CustomCss = value.CustomCss;
            CustomJs = value.CustomJs;
            IsCustomCssEnabled = value.IsCustomCssEnabled;
            IsCustomJsEnabled = value.IsCustomJsEnabled;
            UserAgent = value.UserAgent ?? string.Empty;
            UrlRules = new ObservableCollection<UrlScriptRule>(value.UrlRules ?? new());
        }
        else
        {
            IsEditing = false;
            Title = "Nowa strona";
            Name = string.Empty;
            Url = string.Empty;
            Description = string.Empty;
            IconUrl = string.Empty;
            CustomCss = string.Empty;
            CustomJs = string.Empty;
            IsCustomCssEnabled = true;
            IsCustomJsEnabled = true;
            UserAgent = string.Empty;
            UrlRules = new ObservableCollection<UrlScriptRule>();
        }
        IsAddingUrlRule = false;
    }

    [RelayCommand]
    private void FetchFavicon()
    {
        if (string.IsNullOrWhiteSpace(Url)) return;
        var detected = _webAppService.GetFaviconUrl(Url);
        if (!string.IsNullOrWhiteSpace(detected))
        {
            IconUrl = detected;
        }
    }

    partial void OnUrlChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(IconUrl) && !string.IsNullOrWhiteSpace(value) && value.Length > 8)
        {
            FetchFavicon();
        }
    }

    [RelayCommand]
    private void ApplyCssPreset(ScriptPreset preset)
    {
        if (preset == null) return;
        if (string.IsNullOrWhiteSpace(CustomCss))
        {
            CustomCss = preset.Code;
        }
        else
        {
            CustomCss += "\n\n" + preset.Code;
        }
        IsCustomCssEnabled = true;
    }

    [RelayCommand]
    private void ApplyJsPreset(ScriptPreset preset)
    {
        if (preset == null) return;
        if (string.IsNullOrWhiteSpace(CustomJs))
        {
            CustomJs = preset.Code;
        }
        else
        {
            CustomJs += "\n\n" + preset.Code;
        }
        IsCustomJsEnabled = true;
    }

    // Zarządzanie regułami dla podstron
    [RelayCommand]
    private void ShowAddUrlRule()
    {
        NewRuleName = string.Empty;
        NewRuleUrlPattern = string.Empty;
        NewRuleCustomCss = string.Empty;
        NewRuleCustomJs = string.Empty;
        IsAddingUrlRule = true;
    }

    [RelayCommand]
    private void CancelAddUrlRule()
    {
        IsAddingUrlRule = false;
    }

    [RelayCommand]
    private async Task SaveUrlRule()
    {
        if (string.IsNullOrWhiteSpace(NewRuleUrlPattern))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj fragment lub wzorzec URL (np. /watch, login, article).", "OK");
            return;
        }

        var rule = new UrlScriptRule
        {
            Id = Guid.NewGuid(),
            Name = !string.IsNullOrWhiteSpace(NewRuleName) ? NewRuleName.Trim() : NewRuleUrlPattern.Trim(),
            UrlPattern = NewRuleUrlPattern.Trim(),
            CustomCss = NewRuleCustomCss ?? string.Empty,
            CustomJs = NewRuleCustomJs ?? string.Empty,
            IsEnabled = true
        };

        UrlRules.Add(rule);
        IsAddingUrlRule = false;
    }

    [RelayCommand]
    private void RemoveUrlRule(UrlScriptRule rule)
    {
        if (rule != null && UrlRules.Contains(rule))
        {
            UrlRules.Remove(rule);
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj nazwę aplikacji.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Url))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj poprawny adres URL.", "OK");
            return;
        }

        var cleanUrl = Url.Trim();
        if (!cleanUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !cleanUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            cleanUrl = "https://" + cleanUrl;
        }

        var appToSave = WebApp ?? new WebApp();
        appToSave.Name = Name.Trim();
        appToSave.Url = cleanUrl;
        appToSave.Description = Description.Trim();
        appToSave.IconUrl = !string.IsNullOrWhiteSpace(IconUrl) ? IconUrl.Trim() : _webAppService.GetFaviconUrl(cleanUrl);
        appToSave.CustomCss = CustomCss ?? string.Empty;
        appToSave.CustomJs = CustomJs ?? string.Empty;
        appToSave.IsCustomCssEnabled = IsCustomCssEnabled;
        appToSave.IsCustomJsEnabled = IsCustomJsEnabled;
        appToSave.UserAgent = string.IsNullOrWhiteSpace(UserAgent) ? null : UserAgent.Trim();
        appToSave.UrlRules = UrlRules.ToList();

        if (IsEditing)
        {
            await _webAppService.UpdateAsync(appToSave);
        }
        else
        {
            await _webAppService.AddAsync(appToSave);
        }

        await Shell.Current.GoToAsync("//MainPage");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}
