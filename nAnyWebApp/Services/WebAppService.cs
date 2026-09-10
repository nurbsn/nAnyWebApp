using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using nAnyWebApp.Models;

namespace nAnyWebApp.Services;

public class WebAppService : IWebAppService
{
    private readonly string _storageFile;
    private readonly List<WebApp> _apps = new();
    private readonly object _lock = new();
    private bool _isLoaded;

    public event EventHandler? WebAppsChanged;

    public WebAppService()
    {
        _storageFile = Path.Combine(FileSystem.AppDataDirectory, "nany_webapps.json");
    }

    private void NotifyChanged()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                WebAppsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd powiadomienia WebAppsChanged: {ex.Message}");
            }
        });
    }

    private async Task EnsureLoadedAsync()
    {
        if (_isLoaded) return;

        lock (_lock)
        {
            if (_isLoaded) return;

            try
            {
                if (File.Exists(_storageFile))
                {
                    var json = File.ReadAllText(_storageFile);
                    var loaded = JsonSerializer.Deserialize<List<WebApp>>(json);
                    if (loaded != null && loaded.Count > 0)
                    {
                        _apps.Clear();
                        _apps.AddRange(loaded);
                        _isLoaded = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas odczytu bazy WebApps: {ex.Message}");
            }

            // Inicjalizacja domyślnych aplikacji demonstracyjnych
            SeedDefaultApps();
            _isLoaded = true;
            SaveInternal();
        }

        await Task.CompletedTask;
    }

    private void SeedDefaultApps()
    {
        _apps.Clear();
        _apps.Add(new WebApp
        {
            Id = Guid.NewGuid(),
            Name = "GitHub",
            Url = "https://github.com",
            Description = "Platforma dla deweloperów i repozytoria kodu",
            IconUrl = GetFaviconUrl("https://github.com"),
            CustomCss = "/* Przykład: subtelny zaokrąglony styl kart */\n.Box { border-radius: 12px !important; }",
            CustomJs = "console.log('[nAnyWebApp] GitHub załadowany pomyślnie!');",
            IsCustomCssEnabled = true,
            IsCustomJsEnabled = true,
            CreatedAt = DateTime.UtcNow
        });

        _apps.Add(new WebApp
        {
            Id = Guid.NewGuid(),
            Name = "Wikipedia",
            Url = "https://pl.wikipedia.org",
            Description = "Wolna encyklopedia",
            IconUrl = GetFaviconUrl("https://pl.wikipedia.org"),
            CustomCss = @"/* Wyśrodkowana, czytelna szerokość tekstu */
#content {
    max-width: 960px !important;
    margin: 0 auto !important;
    font-size: 17px !important;
    line-height: 1.65 !important;
}",
            CustomJs = "console.log('[nAnyWebApp] Wikipedia w trybie czytelnym.');",
            IsCustomCssEnabled = true,
            IsCustomJsEnabled = true,
            CreatedAt = DateTime.UtcNow
        });

        _apps.Add(new WebApp
        {
            Id = Guid.NewGuid(),
            Name = "DuckDuckGo",
            Url = "https://duckduckgo.com",
            Description = "Prywatna wyszukiwarka internetowa",
            IconUrl = GetFaviconUrl("https://duckduckgo.com"),
            CustomCss = "",
            CustomJs = "",
            IsCustomCssEnabled = false,
            IsCustomJsEnabled = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    private void SaveInternal()
    {
        try
        {
            var dir = Path.GetDirectoryName(_storageFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(_apps, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_storageFile, json);
            NotifyChanged();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas zapisu bazy WebApps: {ex.Message}");
        }
    }

    public async Task<List<WebApp>> GetAllAsync()
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            return _apps.OrderByDescending(a => a.LastOpenedAt ?? a.CreatedAt).ToList();
        }
    }

    public async Task<WebApp?> GetByIdAsync(Guid id)
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            return _apps.FirstOrDefault(a => a.Id == id);
        }
    }

    public async Task AddAsync(WebApp app)
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            if (string.IsNullOrWhiteSpace(app.IconUrl))
            {
                app.IconUrl = GetFaviconUrl(app.Url);
            }
            _apps.Add(app);
            SaveInternal();
        }
    }

    public async Task UpdateAsync(WebApp app)
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            var index = _apps.FindIndex(a => a.Id == app.Id);
            if (index >= 0)
            {
                if (string.IsNullOrWhiteSpace(app.IconUrl))
                {
                    app.IconUrl = GetFaviconUrl(app.Url);
                }
                _apps[index] = app;
                SaveInternal();
            }
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            var item = _apps.FirstOrDefault(a => a.Id == id);
            if (item != null)
            {
                _apps.Remove(item);
                SaveInternal();
            }
        }
    }

    public async Task RecordOpenedAsync(Guid id)
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            var item = _apps.FirstOrDefault(a => a.Id == id);
            if (item != null)
            {
                item.LastOpenedAt = DateTime.UtcNow;
                SaveInternal();
            }
        }
    }

    public string GetFaviconUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        try
        {
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return $"https://www.google.com/s2/favicons?domain={uri.Host}&sz=128";
            }
        }
        catch
        {
            // Ignore
        }
        return string.Empty;
    }

    public async Task<string> ExportToJsonAsync()
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            return JsonSerializer.Serialize(_apps, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public async Task<bool> ImportFromJsonAsync(string json)
    {
        try
        {
            var imported = JsonSerializer.Deserialize<List<WebApp>>(json);
            if (imported != null && imported.Count > 0)
            {
                await EnsureLoadedAsync();
                lock (_lock)
                {
                    foreach (var app in imported)
                    {
                        var existing = _apps.FirstOrDefault(a => a.Id == app.Id);
                        if (existing != null)
                        {
                            var index = _apps.IndexOf(existing);
                            _apps[index] = app;
                        }
                        else
                        {
                            _apps.Add(app);
                        }
                    }
                    SaveInternal();
                }
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas importu JSON: {ex.Message}");
        }
        return false;
    }
}
