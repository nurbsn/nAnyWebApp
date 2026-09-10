using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using nAnyWebApp.Models;
using nAnyWebApp.Resources.Styles;

namespace nAnyWebApp.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;

        Title = "nAnyWebApp";

        Content = new Grid
        {
            RowDefinitions = Rows.Define(Auto, Auto, Star),
            Padding = new Thickness(16, 12),
            RowSpacing = 12,
            Children =
            {
                // 1. Górny nagłówek z tytułem i przyciskiem dodawania
                new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Auto),
                    Children =
                    {
                        new VerticalStackLayout
                        {
                            Spacing = 2,
                            Children =
                            {
                                new Label
                                {
                                    Text = "nAnyWebApp",
                                    FontAttributes = FontAttributes.Bold,
                                    FontSize = 26,
                                    TextColor = AppColors.Primary
                                },
                                new Label
                                {
                                    Text = "Twoje ulubione strony w postaci aplikacji",
                                    FontSize = 13,
                                    TextColor = Colors.Gray
                                }
                            }
                        }.Column(0),

                        new Button
                        {
                            Text = "+ Dodaj stronę",
                            FontAttributes = FontAttributes.Bold,
                            CornerRadius = 8,
                            BackgroundColor = AppColors.Primary,
                            TextColor = Colors.White,
                            Padding = new Thickness(16, 8)
                        }
                        .Column(1)
                        .CenterVertical()
                        .BindCommand(nameof(MainViewModel.AddNewWebAppCommand))
                    }
                }.Row(0),

                // 2. Pasek wyszukiwania
                new SearchBar
                {
                    Placeholder = "Szukaj aplikacji lub domeny...",
                    FontSize = 14
                }
                .Row(1)
                .Bind(SearchBar.TextProperty, nameof(MainViewModel.SearchText)),

                // 3. Lista aplikacji
                new RefreshView
                {
                    Content = new CollectionView
                    {
                        ItemTemplate = new DataTemplate(BuildWebAppItemTemplate),
                        EmptyView = BuildEmptyView()
                    }
                    .Bind(CollectionView.ItemsSourceProperty, nameof(MainViewModel.WebApps))
                }
                .Row(2)
                .Bind(RefreshView.IsRefreshingProperty, nameof(MainViewModel.IsRefreshing))
                .Bind(RefreshView.CommandProperty, nameof(MainViewModel.LoadWebAppsCommand))
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadWebAppsCommand.Execute(null);
        App.OpenPendingWebAppIfAny();
    }

    private static View BuildEmptyView()
    {
        return new VerticalStackLayout
        {
            Spacing = 16,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(24),
            Children =
            {
                new Label
                {
                    Text = "🌐",
                    FontSize = 54,
                    HorizontalTextAlignment = TextAlignment.Center
                },
                new Label
                {
                    Text = "Brak dodanych stron",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    HorizontalTextAlignment = TextAlignment.Center
                },
                new Label
                {
                    Text = "Nie masz jeszcze żadnych aplikacji. Kliknij przycisk „+ Dodaj stronę” powyżej, aby utworzyć swoją pierwszą aplikację z własnym CSS i JavaScript!",
                    FontSize = 14,
                    TextColor = Colors.Gray,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        };
    }

    private static View BuildWebAppItemTemplate()
    {
        return new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(14) },
            Stroke = Color.FromArgb("#E0E0E0"),
            BackgroundColor = Colors.White,
            Margin = new Thickness(0, 6),
            Padding = new Thickness(14),
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Colors.Black),
                Offset = new Point(0, 2),
                Radius = 4,
                Opacity = 0.08f
            },
            Content = new Grid
            {
                ColumnDefinitions = Columns.Define(Auto, Star, Auto),
                RowDefinitions = Rows.Define(Auto, Auto),
                RowSpacing = 8,
                ColumnSpacing = 12,
                Children =
                {
                    // Kolumna 0: Ikona aplikacji
                    new Border
                    {
                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
                        Stroke = Color.FromArgb("#EBEBEB"),
                        BackgroundColor = Color.FromArgb("#F5F5F7"),
                        WidthRequest = 48,
                        HeightRequest = 48,
                        Content = new Image
                        {
                            Aspect = Aspect.AspectFit,
                            WidthRequest = 36,
                            HeightRequest = 36
                        }.Bind(Image.SourceProperty, nameof(WebApp.IconUrl))
                    }
                    .Row(0)
                    .Column(0)
                    .CenterVertical(),

                    // Kolumna 1: Informacje o aplikacji
                    new VerticalStackLayout
                    {
                        Spacing = 3,
                        Children =
                        {
                            new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 16,
                                TextColor = Color.FromArgb("#1F1F1F"),
                                LineBreakMode = LineBreakMode.TailTruncation
                            }.Bind(Label.TextProperty, nameof(WebApp.Name)),

                            new Label
                            {
                                FontSize = 12,
                                TextColor = Color.FromArgb("#666666"),
                                LineBreakMode = LineBreakMode.TailTruncation
                            }.Bind(Label.TextProperty, nameof(WebApp.Domain)),

                            // Tagi CSS / JS
                            new HorizontalStackLayout
                            {
                                Spacing = 6,
                                Children =
                                {
                                    new Border
                                    {
                                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(4) },
                                        Stroke = Colors.Transparent,
                                        BackgroundColor = Color.FromArgb("#E8F0FE"),
                                        Padding = new Thickness(6, 2),
                                        Content = new Label
                                        {
                                            Text = "CSS",
                                            FontSize = 10,
                                            FontAttributes = FontAttributes.Bold,
                                            TextColor = Color.FromArgb("#1A73E8")
                                        }
                                    }.Bind(Border.IsVisibleProperty, nameof(WebApp.HasCustomCss)),

                                    new Border
                                    {
                                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(4) },
                                        Stroke = Colors.Transparent,
                                        BackgroundColor = Color.FromArgb("#FEF7E0"),
                                        Padding = new Thickness(6, 2),
                                        Content = new Label
                                        {
                                            Text = "JS",
                                            FontSize = 10,
                                            FontAttributes = FontAttributes.Bold,
                                            TextColor = Color.FromArgb("#B06000")
                                        }
                                    }.Bind(Border.IsVisibleProperty, nameof(WebApp.HasCustomJs))
                                }
                            }
                        }
                    }
                    .Row(0)
                    .Column(1)
                    .CenterVertical(),

                    // Kolumna 2: Główny przycisk "Otwórz"
                    new Button
                    {
                        Text = "Otwórz",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 13,
                        CornerRadius = 8,
                        BackgroundColor = AppColors.Primary,
                        TextColor = Colors.White,
                        HeightRequest = 38,
                        Padding = new Thickness(14, 0)
                    }
                    .Row(0)
                    .Column(2)
                    .CenterVertical()
                    .BindCommand(
                        nameof(MainViewModel.OpenWebAppCommand),
                        source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(MainViewModel)),
                        parameterPath: "."
                    ),

                    // Wiersz 1: Pasek dodatkowych akcji (Skrót na pulpit / ekran, Edytuj, Usuń)
                    new BoxView
                    {
                        HeightRequest = 1,
                        Color = Color.FromArgb("#F0F0F0")
                    }
                    .Row(1)
                    .ColumnSpan(3),

                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        HorizontalOptions = LayoutOptions.End,
                        Children =
                        {
                            new Button
                            {
                                Text = "📌 Skrót",
                                FontSize = 12,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#F0F4F8"),
                                TextColor = Color.FromArgb("#333333"),
                                HeightRequest = 32,
                                Padding = new Thickness(10, 0)
                            }.BindCommand(
                                nameof(MainViewModel.CreateShortcutCommand),
                                source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(MainViewModel)),
                                parameterPath: "."
                            ),

                            new Button
                            {
                                Text = "✏️ Edytuj",
                                FontSize = 12,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#F0F4F8"),
                                TextColor = Color.FromArgb("#333333"),
                                HeightRequest = 32,
                                Padding = new Thickness(10, 0)
                            }.BindCommand(
                                nameof(MainViewModel.EditWebAppCommand),
                                source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(MainViewModel)),
                                parameterPath: "."
                            ),

                            new Button
                            {
                                Text = "🗑️ Usuń",
                                FontSize = 12,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#FDE8E8"),
                                TextColor = Color.FromArgb("#C5221F"),
                                HeightRequest = 32,
                                Padding = new Thickness(10, 0)
                            }.BindCommand(
                                nameof(MainViewModel.DeleteWebAppCommand),
                                source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(MainViewModel)),
                                parameterPath: "."
                            )
                        }
                    }
                    .Row(1)
                    .ColumnSpan(3)
                }
            }
        };
    }
}
