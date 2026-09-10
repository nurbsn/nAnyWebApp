using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using nAnyWebApp.Resources.Styles;
using nAnyWebApp.ViewModels;

namespace nAnyWebApp.Views;

public partial class ustawieniaPage : ContentPage
{
    public ustawieniaPage(ustawieniaViewModel viewModel)
    {
        BindingContext = viewModel;
        Title = "Ustawienia";

        Content = new ScrollView
        {
            Padding = new Thickness(16),
            Content = new VerticalStackLayout
            {
                Spacing = 20,
                Children =
                {
                    // Nagłówek
                    new VerticalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Label
                            {
                                Text = "Ustawienia aplikacji",
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 24,
                                TextColor = AppColors.Primary
                            },
                            new Label
                            {
                                Text = "Zarządzanie danymi stron, kopia zapasowa i informacje",
                                FontSize = 13,
                                TextColor = Colors.Gray
                            }
                        }
                    },

                    // SEKCJA 1: Kopia zapasowa / Zarządzanie danymi
                    BuildCardSection("Kopia zapasowa i dane", new VerticalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            new Label
                            {
                                Text = "Możesz wyeksportować wszystkie swoje zapisane strony wraz z kodem CSS i JS do formatu JSON lub zaimportować je na innym urządzeniu.",
                                FontSize = 13,
                                TextColor = Color.FromArgb("#555555")
                            },

                            new Grid
                            {
                                ColumnDefinitions = Columns.Define(Star, Star),
                                ColumnSpacing = 12,
                                Children =
                                {
                                    new Button
                                    {
                                        Text = "📋 Eksportuj (Kopiuj JSON)",
                                        FontSize = 13,
                                        CornerRadius = 8,
                                        BackgroundColor = Color.FromArgb("#F0F4F8"),
                                        TextColor = Color.FromArgb("#1A73E8"),
                                        HeightRequest = 44
                                    }
                                    .Column(0)
                                    .BindCommand(nameof(ustawieniaViewModel.ExportDataCommand)),

                                    new Button
                                    {
                                        Text = "📥 Importuj z JSON",
                                        FontSize = 13,
                                        CornerRadius = 8,
                                        BackgroundColor = Color.FromArgb("#F0F4F8"),
                                        TextColor = Color.FromArgb("#1A73E8"),
                                        HeightRequest = 44
                                    }
                                    .Column(1)
                                    .BindCommand(nameof(ustawieniaViewModel.ImportDataCommand))
                                }
                            }
                        }
                    }),

                    // SEKCJA 2: O aplikacji
                    BuildCardSection("O aplikacji nAnyWebApp", new VerticalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            new Grid
                            {
                                ColumnDefinitions = Columns.Define(Star, Auto),
                                Children =
                                {
                                    new Label
                                    {
                                        Text = "Wersja aplikacji",
                                        FontSize = 14,
                                        TextColor = Color.FromArgb("#333333")
                                    }.Column(0).CenterVertical(),

                                    new Label
                                    {
                                        FontAttributes = FontAttributes.Bold,
                                        FontSize = 14,
                                        TextColor = Colors.Gray
                                    }
                                    .Column(1)
                                    .CenterVertical()
                                    .Bind(Label.TextProperty, nameof(ustawieniaViewModel.AppVersion))
                                }
                            },

                            new BoxView { HeightRequest = 1, Color = Color.FromArgb("#F0F0F0") },

                            new Grid
                            {
                                ColumnDefinitions = Columns.Define(Star, Auto),
                                Children =
                                {
                                    new Label
                                    {
                                        Text = "Platformy docelowe",
                                        FontSize = 14,
                                        TextColor = Color.FromArgb("#333333")
                                    }.Column(0).CenterVertical(),

                                    new Label
                                    {
                                        Text = "Windows & Android (.NET MAUI)",
                                        FontSize = 13,
                                        TextColor = Colors.Gray
                                    }.Column(1).CenterVertical()
                                }
                            },

                            new BoxView { HeightRequest = 1, Color = Color.FromArgb("#F0F0F0") },

                            new Button
                            {
                                Text = "🐙 Odwiedź repozytorium GitHub",
                                FontSize = 13,
                                CornerRadius = 8,
                                BackgroundColor = Color.FromArgb("#24292E"),
                                TextColor = Colors.White,
                                HeightRequest = 44
                            }
                            .BindCommand(nameof(ustawieniaViewModel.OpenGitHubCommand))
                        }
                    })
                }
            }
        };
    }

    private static View BuildCardSection(string title, View content)
    {
        return new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
            Stroke = Color.FromArgb("#E4E4E4"),
            BackgroundColor = Colors.White,
            Padding = new Thickness(16),
            Content = new VerticalStackLayout
            {
                Spacing = 14,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                        TextColor = Color.FromArgb("#222222")
                    },
                    new BoxView
                    {
                        HeightRequest = 1,
                        Color = Color.FromArgb("#F0F0F0")
                    },
                    content
                }
            }
        };
    }
}
