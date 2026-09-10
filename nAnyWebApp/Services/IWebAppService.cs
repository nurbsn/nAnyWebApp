using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using nAnyWebApp.Models;

namespace nAnyWebApp.Services;

public interface IWebAppService
{
    event EventHandler? WebAppsChanged;

    Task<List<WebApp>> GetAllAsync();
    Task<WebApp?> GetByIdAsync(Guid id);
    Task AddAsync(WebApp app);
    Task UpdateAsync(WebApp app);
    Task DeleteAsync(Guid id);
    Task RecordOpenedAsync(Guid id);
    string GetFaviconUrl(string url);
    Task<string> ExportToJsonAsync();
    Task<bool> ImportFromJsonAsync(string json);
}
