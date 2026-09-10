#if WINDOWS
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using nAnyWebApp.Models;

namespace nAnyWebApp.Services;

public class WindowsShortcutService : IShortcutService
{
    public bool IsSupported => true;

    public Task<bool> CreateShortcutAsync(WebApp app)
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string safeName = SanitizeFileName(app.Name);
            if (string.IsNullOrWhiteSpace(safeName)) safeName = "WebApp";
            string shortcutPath = Path.Combine(desktopPath, $"{safeName}.lnk");

            string exePath = Environment.ProcessPath ?? "";
            if (string.IsNullOrEmpty(exePath))
            {
                using var process = Process.GetCurrentProcess();
                exePath = process.MainModule?.FileName ?? "";
            }

            Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType != null)
            {
                dynamic shell = Activator.CreateInstance(shellType)!;
                dynamic shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = exePath;
                shortcut.Arguments = $"--webapp-id {app.Id}";
                shortcut.Description = $"Uruchom {app.Name} w nAnyWebApp";
                shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                shortcut.Save();
                return Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas tworzenia skrótu Windows: {ex.Message}");
        }

        return Task.FromResult(false);
    }

    private static string SanitizeFileName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var chars = name.Where(c => !invalidChars.Contains(c)).ToArray();
        return new string(chars).Trim();
    }
}
#endif
