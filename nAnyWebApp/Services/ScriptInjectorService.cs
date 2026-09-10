using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using nAnyWebApp.Models;

namespace nAnyWebApp.Services;

public class ScriptInjectorService : IScriptInjectorService
{
    public async Task<bool> InjectAllAsync(WebView webView, WebApp app, string? currentUrl = null)
    {
        if (webView == null || app == null) return false;

        bool overallSuccess = true;

        // 1. Wstrzykiwanie globalnych stylów CSS
        if (app.IsCustomCssEnabled && !string.IsNullOrWhiteSpace(app.CustomCss))
        {
            var res = await InjectCssAsync(webView, app.CustomCss, "__nany_custom_css");
            if (!res) overallSuccess = false;
        }

        // 2. Wstrzykiwanie globalnego kodu JavaScript
        if (app.IsCustomJsEnabled && !string.IsNullOrWhiteSpace(app.CustomJs))
        {
            var res = await InjectJavaScriptAsync(webView, app.CustomJs, "__nany_custom_js");
            if (!res) overallSuccess = false;
        }

        // 3. Wstrzykiwanie reguł dedykowanych dla podstron / fragmentów URL
        if (app.UrlRules != null && app.UrlRules.Count > 0)
        {
            string urlToCheck = currentUrl ?? string.Empty;

            foreach (var rule in app.UrlRules)
            {
                if (!rule.IsEnabled) continue;
                if (string.IsNullOrWhiteSpace(rule.UrlPattern)) continue;

                bool matches = IsUrlMatching(urlToCheck, rule.UrlPattern);
                if (matches)
                {
                    if (!string.IsNullOrWhiteSpace(rule.CustomCss))
                    {
                        await InjectCssAsync(webView, rule.CustomCss, $"__nany_rule_css_{rule.Id:N}");
                    }

                    if (!string.IsNullOrWhiteSpace(rule.CustomJs))
                    {
                        await InjectJavaScriptAsync(webView, rule.CustomJs, $"__nany_rule_js_{rule.Id:N}");
                    }
                }
            }
        }

        return overallSuccess;
    }

    private static bool IsUrlMatching(string currentUrl, string pattern)
    {
        if (string.IsNullOrWhiteSpace(currentUrl) || string.IsNullOrWhiteSpace(pattern))
            return false;

        var cleanPattern = pattern.Trim();

        // Jeśli wzorzec to *, pasuje do wszystkiego
        if (cleanPattern == "*") return true;

        // Sprawdzenie czy adres URL zawiera podany fragment
        return currentUrl.Contains(cleanPattern, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> InjectCssAsync(WebView webView, string css, string styleId = "__nany_custom_css")
    {
        if (webView == null || string.IsNullOrWhiteSpace(css)) return false;

        try
        {
            string base64Css = Convert.ToBase64String(Encoding.UTF8.GetBytes(css));
            string script = $@"
(function() {{
    try {{
        var b64 = '{base64Css}';
        var bin = window.atob(b64);
        var bytes = new Uint8Array(bin.length);
        for (var i = 0; i < bin.length; i++) {{
            bytes[i] = bin.charCodeAt(i);
        }}
        var cssContent = new TextDecoder('utf-8').decode(bytes);
        
        var styleId = '{styleId}';
        var styleEl = document.getElementById(styleId);
        if (!styleEl) {{
            styleEl = document.createElement('style');
            styleEl.id = styleId;
            styleEl.type = 'text/css';
            (document.head || document.documentElement).appendChild(styleEl);
        }}
        styleEl.textContent = cssContent;
        return true;
    }} catch (e) {{
        console.error('[nAnyWebApp] CSS injection failed:', e);
        return false;
    }}
}})();";

            var result = await webView.EvaluateJavaScriptAsync(script);
            return result != null && !result.Equals("false", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas wstrzykiwania CSS ({styleId}): {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InjectJavaScriptAsync(WebView webView, string javascript, string scriptId = "__nany_custom_js")
    {
        if (webView == null || string.IsNullOrWhiteSpace(javascript)) return false;

        try
        {
            string base64Js = Convert.ToBase64String(Encoding.UTF8.GetBytes(javascript));
            string script = $@"
(function() {{
    try {{
        var b64 = '{base64Js}';
        var bin = window.atob(b64);
        var bytes = new Uint8Array(bin.length);
        for (var i = 0; i < bin.length; i++) {{
            bytes[i] = bin.charCodeAt(i);
        }}
        var jsContent = new TextDecoder('utf-8').decode(bytes);

        var scriptId = '{scriptId}';
        var oldScript = document.getElementById(scriptId);
        if (oldScript) {{
            oldScript.remove();
        }}

        var newScript = document.createElement('script');
        newScript.id = scriptId;
        newScript.type = 'text/javascript';
        newScript.textContent = jsContent;
        (document.body || document.head || document.documentElement).appendChild(newScript);
        return true;
    }} catch (e) {{
        console.error('[nAnyWebApp] JS injection failed:', e);
        return false;
    }}
}})();";

            var result = await webView.EvaluateJavaScriptAsync(script);
            return result != null && !result.Equals("false", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas wstrzykiwania JS ({scriptId}): {ex.Message}");
            return false;
        }
    }
}
