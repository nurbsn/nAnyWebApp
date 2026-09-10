using System.Threading.Tasks;
using nAnyWebApp.Models;

namespace nAnyWebApp.Services;

public class DefaultShortcutService : IShortcutService
{
    public bool IsSupported => false;

    public Task<bool> CreateShortcutAsync(WebApp app)
    {
        return Task.FromResult(false);
    }
}
