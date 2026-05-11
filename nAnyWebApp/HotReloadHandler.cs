using System.Diagnostics.CodeAnalysis;

namespace nAnyWebApp;

class HotReloadHandler : ICommunityToolkitHotReloadHandler
{
	public async void OnHotReload(IReadOnlyList<Type> types)
	{
		if (Application.Current?.Windows is null)
		{
			Trace.WriteLine($"{nameof(HotReloadHandler)} Failed: {nameof(Application)}.{nameof(Application.Current)}.{nameof(Application.Current.Windows)} is null");
			return;
		}

		foreach (var window in Application.Current.Windows)
		{
			if (window.Page is not Page currentPage)
			{
				return;
			}

			foreach (var type in types)
			{
				if (type.IsSubclassOf(typeof(Page)))
				{
					if (window.Page is AppShell shell)
					{
						if (shell.CurrentPage is Page visiblePage
							&& visiblePage.GetType() == type)
						{
							var currentPageShellRoute = AppShell.GetRoute(type);

							await currentPage.Dispatcher.DispatchAsync(async () =>
							{
								// Remove the current page (with the old version of the content)
								shell.Navigation.RemovePage(visiblePage);
								// Replace with the new version of the page
								await shell.GoToAsync(currentPageShellRoute, animate: false);
							});

							break;
						}
					}
					else
					{
						if (TryGetModalStackPage(window, out var modalPage))
						{
							await currentPage.Dispatcher.DispatchAsync(async () =>
							{
								await currentPage.Navigation.PopModalAsync(animated: false);
								await currentPage.Navigation.PushModalAsync(modalPage, animated: false);
							});
						}
						else
						{
							await currentPage.Dispatcher.DispatchAsync(async () =>
							{
								await currentPage.Navigation.PopAsync(animated: false);
								await currentPage.Navigation.PushAsync(modalPage, animated: false);
							});
						}

						break;
					}
				}
			}
		}
	}

	static bool TryGetModalStackPage(Window window, [NotNullWhen(true)] out Page? page)
	{
		page = window.Navigation.ModalStack.LastOrDefault();
		return page is not null;
	}
}
