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

		builder.Services.AddTransient<AppShell>();

		builder.Services.AddSingleton<App>();
#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<ICommunityToolkitHotReloadHandler, HotReloadHandler>();

		builder.Services.AddTransient<MainPage, MainViewModel>();

		builder.Services.AddTransient<nWebViewPage, nWebViewViewModel>();

		builder.Services.AddTransient<ustawieniaPage, ustawieniaViewModel>();

		builder.Services.UsePageResolver();

		return builder.Build();
	}
}
