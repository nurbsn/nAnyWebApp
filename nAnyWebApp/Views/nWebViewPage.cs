using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace nAnyWebApp.Views;

public partial class nWebViewPage : ContentPage
{
	public nWebViewPage(nWebViewViewModel viewModel)
	{
		BindingContext = viewModel;

		Content = new Grid
		{
			RowDefinitions = Rows.Define(Star, Auto),
			Children =
			{
				new WebView().Assign(out WebView webView)
							 .Bind(WebView.SourceProperty, nameof(nWebViewViewModel.Source)),
				new ActivityIndicator()
					.CenterHorizontal()
					.CenterVertical()
					.Bind(ActivityIndicator.IsVisibleProperty, nameof(nWebViewViewModel.IsLoading), mode: BindingMode.OneWay)
					.Bind(ActivityIndicator.IsRunningProperty, nameof(nWebViewViewModel.IsLoading), mode: BindingMode.OneWay),
				new HorizontalStackLayout
				{
					Spacing = 12,
					Children =
					{
						new Button().Text("Back")
									.Bind(Button.IsEnabledProperty, WebView.CanGoBackProperty.PropertyName, source: webView)
									.BindCommand(nameof(nWebViewViewModel.NavigateBackCommand), parameterSource: webView),
						new Button().Text("Forward")
									.Bind(Button.IsEnabledProperty, WebView.CanGoForwardProperty.PropertyName, source: webView)
									.BindCommand(nameof(nWebViewViewModel.NavigateForwardCommand), parameterSource: webView),
						new Button().Text("Refresh")
									.BindCommand(nameof(nWebViewViewModel.RefreshPageCommand), parameterSource: webView),
						new Button().Text("Open in browser")
									.BindCommand(nameof(nWebViewViewModel.OpenInBrowserCommand)),
					},
				}.CenterHorizontal()
				.Row(1),
			},
		};

		webView.Navigated += (sender, e) => { viewModel.WebViewNavigatedCommand.Execute(e); };
	}
}
