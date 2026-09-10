using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using nAnyWebApp.Resources.Styles;
using nAnyWebApp.ViewModels;

namespace nAnyWebApp.Views;

public partial class nWebViewPage : ContentPage
{
    private readonly nWebViewViewModel _viewModel;
    private readonly WebView _webView;

    public nWebViewPage(nWebViewViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;

        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);

        _webView = new WebView();
        _webView.Bind(WebView.SourceProperty, nameof(nWebViewViewModel.Source));

        _webView.Navigated += (sender, e) =>
        {
            _viewModel.WebViewNavigatedCommand.Execute(new Tuple<WebNavigatedEventArgs, WebView>(e, _webView));
        };

        Content = new Grid
        {
            RowDefinitions = Rows.Define(Auto, Star),
            Children =
            {
                // 1. Górny pasek narzędzi (ukrywalny w trybie pełnoekranowym)
                BuildToolbar(_webView)
                    .Row(0)
                    .Bind(Border.IsVisibleProperty, nameof(nWebViewViewModel.IsToolbarVisible)),

                // 2. Kontrolka WebView
                _webView.Row(1),

                // 3. Wskaźnik ładowania strony
                new Border
                {
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                    Stroke = Colors.Transparent,
                    BackgroundColor = Color.FromRgba(0, 0, 0, 0.65),
                    Padding = new Thickness(18, 14),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = new VerticalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new ActivityIndicator
                            {
                                Color = Colors.White,
                                IsRunning = true,
                                WidthRequest = 36,
                                HeightRequest = 36
                            },
                            new Label
                            {
                                Text = "Ładowanie...",
                                TextColor = Colors.White,
                                FontSize = 13,
                                HorizontalTextAlignment = TextAlignment.Center
                            }
                        }
                    }
                }
                .Row(1)
                .Bind(Border.IsVisibleProperty, nameof(nWebViewViewModel.IsLoading), mode: BindingMode.OneWay),

                // 4. Pływający miniaturowy przycisk przywrócenia paska w trybie pełnoekranowym
                new Button
                {
                    Text = "⚙️",
                    FontSize = 14,
                    CornerRadius = 18,
                    WidthRequest = 36,
                    HeightRequest = 36,
                    BackgroundColor = Color.FromRgba(40, 40, 40, 0.4),
                    TextColor = Colors.White,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0, 10, 10, 0),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start
                }
                .Row(1)
                .BindCommand(nameof(nWebViewViewModel.ToggleFullscreenCommand))
                .Bind(
                    Button.IsVisibleProperty,
                    nameof(nWebViewViewModel.IsToolbarVisible),
                    convert: (bool isToolbarVisible) => !isToolbarVisible
                )
            }
        };
    }

    private View BuildToolbar(WebView webView)
    {
        return new Border
        {
            Stroke = Color.FromArgb("#E0E0E0"),
            BackgroundColor = Color.FromArgb("#F8F9FA"),
            Padding = new Thickness(10, 6),
            Content = new Grid
            {
                ColumnDefinitions = Columns.Define(Auto, Star, Auto),
                ColumnSpacing = 8,
                Children =
                {
                    // Lewa strona: Nawigacja Wstecz / Dalej / Odśwież / Zamknij
                    new HorizontalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Button
                            {
                                Text = "☰ Menu",
                                FontSize = 12,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#EAEAEA"),
                                TextColor = Color.FromArgb("#333333"),
                                Padding = new Thickness(8, 4)
                            }
                            .BindCommand(nameof(nWebViewViewModel.ToggleMenuCommand)),

                            new Button
                            {
                                Text = "📑 Strony",
                                FontSize = 12,
                                CornerRadius = 6,
                                BackgroundColor = Colors.Transparent,
                                TextColor = Color.FromArgb("#444444"),
                                Padding = new Thickness(6, 4)
                            }
                            .BindCommand(nameof(nWebViewViewModel.CloseCommand)),

                            new Button
                            {
                                Text = "⬅️",
                                FontSize = 14,
                                BackgroundColor = Colors.Transparent,
                                Padding = new Thickness(6, 4)
                            }
                            .Bind(Button.IsEnabledProperty, WebView.CanGoBackProperty.PropertyName, source: webView)
                            .BindCommand(nameof(nWebViewViewModel.NavigateBackCommand), parameterSource: webView),

                            new Button
                            {
                                Text = "➡️",
                                FontSize = 14,
                                BackgroundColor = Colors.Transparent,
                                Padding = new Thickness(6, 4)
                            }
                            .Bind(Button.IsEnabledProperty, WebView.CanGoForwardProperty.PropertyName, source: webView)
                            .BindCommand(nameof(nWebViewViewModel.NavigateForwardCommand), parameterSource: webView),

                            new Button
                            {
                                Text = "🔄",
                                FontSize = 14,
                                BackgroundColor = Colors.Transparent,
                                Padding = new Thickness(6, 4)
                            }
                            .BindCommand(nameof(nWebViewViewModel.RefreshPageCommand), parameterSource: webView)
                        }
                    }
                    .Column(0)
                    .CenterVertical(),

                    // Środek: Tytuł strony / URL
                    new Label
                    {
                        FontSize = 13,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.TailTruncation
                    }
                    .Column(1)
                    .CenterVertical()
                    .Bind(Label.TextProperty, nameof(nWebViewViewModel.PageTitle)),

                    // Prawa strona: Narzędzia (Wstrzykiwanie, Skrót, Pełny ekran)
                    new HorizontalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Button
                            {
                                Text = "⚡ Skrypty",
                                FontSize = 12,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#EEF3FD"),
                                TextColor = Color.FromArgb("#1A73E8"),
                                Padding = new Thickness(8, 4)
                            }
                            .BindCommand(nameof(nWebViewViewModel.ReinjectScriptsCommand), parameterSource: webView),

                            new Button
                            {
                                Text = "📌 Skrót",
                                FontSize = 12,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#F0F0F0"),
                                TextColor = Color.FromArgb("#333333"),
                                Padding = new Thickness(8, 4)
                            }
                            .BindCommand(nameof(nWebViewViewModel.CreateShortcutCommand)),

                            new Button
                            {
                                Text = "⛶ Pełny ekran",
                                FontSize = 12,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#F0F0F0"),
                                TextColor = Color.FromArgb("#333333"),
                                Padding = new Thickness(8, 4)
                            }
                            .BindCommand(nameof(nWebViewViewModel.ToggleFullscreenCommand))
                        }
                    }
                    .Column(2)
                    .CenterVertical()
                }
            }
        };
    }
}
