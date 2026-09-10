using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Syncfusion.Maui.Toolkit.Hosting;
using Maui.Plugins.PageResolver;
using nAnyWebApp.Services;
using nAnyWebApp.ViewModels;
using nAnyWebApp.Views;

namespace nAnyWebApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionToolkit()
			.UseMauiCommunityToolkitMarkup()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Usługi aplikacji
		builder.Services.AddSingleton<IWebAppService, WebAppService>();
		builder.Services.AddSingleton<IScriptInjectorService, ScriptInjectorService>();

#if WINDOWS
		builder.Services.AddSingleton<IShortcutService, WindowsShortcutService>();
#elif ANDROID
		builder.Services.AddSingleton<IShortcutService, AndroidShortcutService>();
#else
		builder.Services.AddSingleton<IShortcutService, DefaultShortcutService>();
#endif

		builder.Services.AddTransient<AppShell>();
		builder.Services.AddSingleton<App>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<ICommunityToolkitHotReloadHandler, HotReloadHandler>();

		// Strony i ViewModele
		builder.Services.AddTransient<MainPage, MainViewModel>();
		builder.Services.AddTransient<nWebViewPage, nWebViewViewModel>();
		builder.Services.AddTransient<WebAppEditPage, WebAppEditViewModel>();
		builder.Services.AddTransient<ustawieniaPage, ustawieniaViewModel>();

		builder.Services.UsePageResolver();

		return builder.Build();
	}
}
