using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace nAnyWebApp;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        HandleIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        HandleIntent(intent);
    }

    private void HandleIntent(Intent? intent)
    {
        if (intent == null) return;

        var webAppId = intent.GetStringExtra("webapp_id");
        if (!string.IsNullOrEmpty(webAppId))
        {
            App.PendingWebAppId = webAppId;
            App.OpenPendingWebAppIfAny();
        }
    }
}
