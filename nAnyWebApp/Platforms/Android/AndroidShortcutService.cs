#if ANDROID
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using nAnyWebApp.Models;

namespace nAnyWebApp.Services;

public class AndroidShortcutService : IShortcutService
{
    public bool IsSupported
    {
        get
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var context = Android.App.Application.Context;
                var shortcutManager = (ShortcutManager?)context.GetSystemService(Java.Lang.Class.FromType(typeof(ShortcutManager)));
                return shortcutManager?.IsRequestPinShortcutSupported ?? false;
            }
            return false;
        }
    }

    public Task<bool> CreateShortcutAsync(WebApp app)
    {
        try
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var context = Android.App.Application.Context;
                var shortcutManager = (ShortcutManager?)context.GetSystemService(Java.Lang.Class.FromType(typeof(ShortcutManager)));

                if (shortcutManager == null || !shortcutManager.IsRequestPinShortcutSupported)
                {
                    return Task.FromResult(false);
                }

                var intent = new Intent(context, typeof(MainActivity));
                intent.SetAction(Intent.ActionView);
                intent.PutExtra("webapp_id", app.Id.ToString());
                intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);

                int iconRes = context.ApplicationInfo?.Icon != 0 ? context.ApplicationInfo!.Icon : Android.Resource.Drawable.SymDefAppIcon;
                var icon = Icon.CreateWithResource(context, iconRes);

                var shortcut = new ShortcutInfo.Builder(context, "nany_webapp_" + app.Id.ToString())
                    .SetShortLabel(app.Name)
                    .SetLongLabel(string.IsNullOrWhiteSpace(app.Description) ? app.Name : $"{app.Name} - {app.Description}")
                    .SetIntent(intent)
                    .SetIcon(icon)
                    .Build();

                bool result = shortcutManager.RequestPinShortcut(shortcut, null);
                return Task.FromResult(result);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Błąd podczas tworzenia skrótu Android: {ex.Message}");
        }

        return Task.FromResult(false);
    }
}
#endif
