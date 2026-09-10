using System.Threading.Tasks;
using nAnyWebApp.Models;

namespace nAnyWebApp.Services;

public interface IShortcutService
{
    bool IsSupported { get; }
    Task<bool> CreateShortcutAsync(WebApp app);
}
