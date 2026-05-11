using System;

namespace nAnyWebApp;

public class AppShell : Shell
{
	public AppShell(MainPage Main, nWebViewPage nWebView, ustawieniaPage ustawienia)
	{


		Items.Add(new ShellContent { Title = "Main", Icon = ImageSource.FromFile("iconblank.png"), Content = Main });

		Items.Add(new ShellContent { Title = "nWebView", Icon = ImageSource.FromFile("iconwebview.png"), Content = nWebView });

		Items.Add(new ShellContent { Title = "ustawienia", Icon = ImageSource.FromFile("iconblank.png"), Content = ustawienia });

		// Explicitly register routes for pages so can force navigating to them to force HotReload
		Routing.RegisterRoute(GetRoute(typeof(MainPage)), typeof(MainPage));
		Routing.RegisterRoute(GetRoute(typeof(nWebViewPage)), typeof(nWebViewPage));
		Routing.RegisterRoute(GetRoute(typeof(ustawieniaPage)), typeof(ustawieniaPage));
	}

	// TODO: Update routing formatting as necessary for your app
	public static string GetRoute(Type pageType) => $"//{pageType.Name}";
}
