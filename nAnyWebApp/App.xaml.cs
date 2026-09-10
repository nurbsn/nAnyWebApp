using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using nAnyWebApp.Views;

namespace nAnyWebApp;

public partial class App : Application
{
	public static string? PendingWebAppId { get; set; }

	readonly AppShell shell;

	public App(AppShell appShell)
	{
		InitializeComponent();
		shell = appShell;

		CheckCommandLineArgs();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(shell);
	}

	private void CheckCommandLineArgs()
	{
		try
		{
			var args = Environment.GetCommandLineArgs();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "--webapp-id" && i + 1 < args.Length)
				{
					PendingWebAppId = args[i + 1];
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Błąd odczytu parametrów startowych: {ex.Message}");
		}
	}

	public static async void OpenPendingWebAppIfAny()
	{
		if (string.IsNullOrWhiteSpace(PendingWebAppId)) return;

		var id = PendingWebAppId;
		PendingWebAppId = null;

		try
		{
			if (Shell.Current != null)
			{
				if (Guid.TryParse(id, out var guid))
				{
					await Shell.Current.GoToAsync($"//webapp_{guid:N}");
				}
				else
				{
					await Shell.Current.GoToAsync($"{nameof(nWebViewPage)}?WebAppId={id}");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Błąd przejścia do aplikacji ze skrótu: {ex.Message}");
		}
	}
}
