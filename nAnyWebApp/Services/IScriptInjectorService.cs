using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using nAnyWebApp.Models;

namespace nAnyWebApp.Services;

public interface IScriptInjectorService
{
    Task<bool> InjectAllAsync(WebView webView, WebApp app, string? currentUrl = null);
    Task<bool> InjectCssAsync(WebView webView, string css, string styleId = "__nany_custom_css");
    Task<bool> InjectJavaScriptAsync(WebView webView, string javascript, string scriptId = "__nany_custom_js");
}
