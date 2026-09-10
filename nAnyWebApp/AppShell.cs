using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using nAnyWebApp.Models;
using nAnyWebApp.Services;
using nAnyWebApp.ViewModels;
using nAnyWebApp.Views;

namespace nAnyWebApp;

public class AppShell : Shell
{
	private readonly IWebAppService _webAppService;
	private readonly IServiceProvider _serviceProvider;
	private readonly MainPage _mainPage;
	private readonly ustawieniaPage _settingsPage;
	private bool _initialSelectionDone;

	public AppShell(
		IWebAppService webAppService,
		IServiceProvider serviceProvider,
		MainPage mainPage,
		ustawieniaPage settingsPage)
	{
		_webAppService = webAppService;
		_serviceProvider = serviceProvider;
		_mainPage = mainPage;
		_settingsPage = settingsPage;

		FlyoutBehavior = FlyoutBehavior.Flyout;

		// Rejestracja tras nawigacji
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(nWebViewPage), typeof(nWebViewPage));
		Routing.RegisterRoute(nameof(WebAppEditPage), typeof(WebAppEditPage));
		Routing.RegisterRoute(nameof(ustawieniaPage), typeof(ustawieniaPage));

		// Ustawienie początkowych pozycji w menu
		SetupInitialItems();

		// Subskrypcja na dynamiczne zmiany w liście stron
		_webAppService.WebAppsChanged += OnWebAppsChanged;

		// Asynchroniczne załadowanie pełnego menu z bazy
		_ = RefreshFlyoutMenuAsync();
	}

	private void SetupInitialItems()
	{
		var mainContent = new ShellContent
		{
			Title = "📑 Strony",
			Route = "MainPage",
			Icon = ImageSource.FromFile("iconblank.png"),
			Content = _mainPage
		};

		var mainFlyout = new FlyoutItem
		{
			Title = "📑 Strony",
			Route = "flyout_main",
			Icon = ImageSource.FromFile("iconblank.png")
		};
		mainFlyout.Items.Add(mainContent);
		Items.Add(mainFlyout);

		var settingsContent = new ShellContent
		{
			Title = "⚙️ Ustawienia",
			Route = "ustawieniaPage",
			Icon = ImageSource.FromFile("iconblank.png"),
			Content = _settingsPage
		};

		var settingsFlyout = new FlyoutItem
		{
			Title = "⚙️ Ustawienia",
			Route = "flyout_settings",
			Icon = ImageSource.FromFile("iconblank.png")
		};
		settingsFlyout.Items.Add(settingsContent);
		Items.Add(settingsFlyout);
	}

	private async void OnWebAppsChanged(object? sender, EventArgs e)
	{
		await RefreshFlyoutMenuAsync();
	}

	public async Task RefreshFlyoutMenuAsync()
	{
		var apps = await _webAppService.GetAllAsync();

		MainThread.BeginInvokeOnMainThread(() =>
		{
			try
			{
				Items.Clear();

				// 1. Każda dodana strona jako osobna pozycja w menu (WebView)
				foreach (var app in apps)
				{
					ImageSource iconSource = ImageSource.FromFile("iconwebview.png");
					if (!string.IsNullOrWhiteSpace(app.IconUrl) &&
						Uri.TryCreate(app.IconUrl, UriKind.Absolute, out var uri))
					{
						iconSource = ImageSource.FromUri(uri);
					}

					var appShellContent = new ShellContent
					{
						Title = app.Name,
						Route = $"webapp_{app.Id:N}",
						Icon = iconSource,
						ContentTemplate = new DataTemplate(() =>
						{
							var vm = _serviceProvider.GetRequiredService<nWebViewViewModel>();
							vm.WebApp = app;
							return new nWebViewPage(vm);
						})
					};

					var flyoutItem = new FlyoutItem
					{
						Title = app.Name,
						Route = $"flyout_{app.Id:N}",
						Icon = iconSource
					};
					flyoutItem.Items.Add(appShellContent);
					Items.Add(flyoutItem);
				}

				// 2. Pozycja: Strony (Panel zarządzania)
				var mainContent = new ShellContent
				{
					Title = "📑 Strony",
					Route = "MainPage",
					Icon = ImageSource.FromFile("iconblank.png"),
					Content = _mainPage
				};

				var mainFlyout = new FlyoutItem
				{
					Title = "📑 Strony",
					Route = "flyout_main",
					Icon = ImageSource.FromFile("iconblank.png")
				};
				mainFlyout.Items.Add(mainContent);
				Items.Add(mainFlyout);

				// 3. Pozycja: Ustawienia
				var settingsContent = new ShellContent
				{
					Title = "⚙️ Ustawienia",
					Route = "ustawieniaPage",
					Icon = ImageSource.FromFile("iconblank.png"),
					Content = _settingsPage
				};

				var settingsFlyout = new FlyoutItem
				{
					Title = "⚙️ Ustawienia",
					Route = "flyout_settings",
					Icon = ImageSource.FromFile("iconblank.png")
				};
				settingsFlyout.Items.Add(settingsContent);
				Items.Add(settingsFlyout);

				// Przy pierwszym załadowaniu bez skrótu ustaw MainPage jako aktywną stronę
				if (!_initialSelectionDone)
				{
					_initialSelectionDone = true;
					if (string.IsNullOrEmpty(App.PendingWebAppId))
					{
						CurrentItem = mainFlyout;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Błąd podczas odświeżania menu Shell: {ex.Message}");
			}
		});
	}

	public static string GetRoute(Type pageType) => $"//{pageType.Name}";
}
