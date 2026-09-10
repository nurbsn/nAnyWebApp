using System;
using System.Collections.Generic;

namespace nAnyWebApp.Models;

public class UrlScriptRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string UrlPattern { get; set; } = string.Empty;
    public string CustomCss { get; set; } = string.Empty;
    public string CustomJs { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}

public class WebApp
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string CustomCss { get; set; } = string.Empty;
    public string CustomJs { get; set; } = string.Empty;
    public bool IsCustomCssEnabled { get; set; } = true;
    public bool IsCustomJsEnabled { get; set; } = true;
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastOpenedAt { get; set; }

    public List<UrlScriptRule> UrlRules { get; set; } = new();

    public string Domain
    {
        get
        {
            if (Uri.TryCreate(Url, UriKind.Absolute, out var uri))
            {
                return uri.Host;
            }
            return Url;
        }
    }

    public string DisplayInitial =>
        !string.IsNullOrWhiteSpace(Name) ? Name.Substring(0, 1).ToUpperInvariant() : "W";

    public bool HasCustomCss => IsCustomCssEnabled && !string.IsNullOrWhiteSpace(CustomCss);
    public bool HasCustomJs => IsCustomJsEnabled && !string.IsNullOrWhiteSpace(CustomJs);
}

// Kompatybilność wsteczna
public class WebApps : WebApp
{
}
