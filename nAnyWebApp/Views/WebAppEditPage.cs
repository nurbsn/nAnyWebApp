using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using nAnyWebApp.Models;
using nAnyWebApp.Resources.Styles;
using nAnyWebApp.ViewModels;
using Switch = Microsoft.Maui.Controls.Switch;

namespace nAnyWebApp.Views;

public partial class WebAppEditPage : ContentPage
{
    public WebAppEditPage(WebAppEditViewModel viewModel)
    {
        BindingContext = viewModel;

        this.Bind(Page.TitleProperty, nameof(WebAppEditViewModel.Title));

        Content = new ScrollView
        {
            Padding = new Thickness(16),
            Content = new VerticalStackLayout
            {
                Spacing = 20,
                Children =
                {
                    // SEKCJA 1: Podstawowe informacje
                    BuildCardSection("Informacje podstawowe", new VerticalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            BuildFormField("Nazwa aplikacji *",
                                new Entry { Placeholder = "np. GitHub, YouTube, Wiadomości" }
                                    .Bind(Entry.TextProperty, nameof(WebAppEditViewModel.Name))),

                            BuildFormField("Główny adres URL *",
                                new Entry { Placeholder = "https://example.com", Keyboard = Keyboard.Url }
                                    .Bind(Entry.TextProperty, nameof(WebAppEditViewModel.Url))),

                            BuildFormField("Krótki opis (opcjonalnie)",
                                new Entry { Placeholder = "Opis widoczny na liście stron" }
                                    .Bind(Entry.TextProperty, nameof(WebAppEditViewModel.Description))),

                            // Ikona z podglądem i przyciskiem auto-pobierania
                            BuildFormField("Adres ikony / Favicon",
                                new Grid
                                {
                                    ColumnDefinitions = Columns.Define(Star, Auto, Auto),
                                    ColumnSpacing = 8,
                                    Children =
                                    {
                                        new Entry { Placeholder = "https://.../favicon.png", Keyboard = Keyboard.Url }
                                            .Column(0)
                                            .Bind(Entry.TextProperty, nameof(WebAppEditViewModel.IconUrl)),

                                        new Button
                                        {
                                            Text = "Pobierz favicon",
                                            FontSize = 12,
                                            CornerRadius = 6,
                                            BackgroundColor = Color.FromArgb("#EAEAEA"),
                                            TextColor = Color.FromArgb("#333333")
                                        }
                                        .Column(1)
                                        .CenterVertical()
                                        .BindCommand(nameof(WebAppEditViewModel.FetchFaviconCommand)),

                                        new Border
                                        {
                                            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(6) },
                                            Stroke = Color.FromArgb("#CCCCCC"),
                                            WidthRequest = 36,
                                            HeightRequest = 36,
                                            Content = new Image
                                            {
                                                Aspect = Aspect.AspectFit
                                            }.Bind(Image.SourceProperty, nameof(WebAppEditViewModel.IconUrl))
                                        }
                                        .Column(2)
                                        .CenterVertical()
                                    }
                                })
                        }
                    }),

                    // SEKCJA 2: Globalne wstrzykiwanie CSS
                    BuildCardSection("Globalne style CSS (dla całej domeny)", new VerticalStackLayout
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
                                        Text = "Włącz globalny arkusz stylów CSS dla tej witryny",
                                        FontSize = 13,
                                        TextColor = Color.FromArgb("#555555")
                                    }.Column(0).CenterVertical(),

                                    new Switch()
                                        .Column(1)
                                        .Bind(Switch.IsToggledProperty, nameof(WebAppEditViewModel.IsCustomCssEnabled))
                                }
                            },

                            new Label
                            {
                                Text = "Gotowe szablony (kliknij, aby wstawić kod):",
                                FontSize = 12,
                                TextColor = Colors.Gray
                            },

                            BuildPresetsRow(viewModel.CssPresets, nameof(WebAppEditViewModel.ApplyCssPresetCommand)),

                            new Editor
                            {
                                Placeholder = "/* Wpisz własne reguły CSS... np. body { background-color: #000 !important; } */",
                                HeightRequest = 120,
                                FontFamily = "Consolas, monospace",
                                FontSize = 13,
                                BackgroundColor = Color.FromArgb("#F9F9FB")
                            }.Bind(Editor.TextProperty, nameof(WebAppEditViewModel.CustomCss))
                        }
                    }),

                    // SEKCJA 3: Globalne wstrzykiwanie JavaScript
                    BuildCardSection("Globalny kod JavaScript (dla całej domeny)", new VerticalStackLayout
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
                                        Text = "Włącz globalny kod JavaScript po załadowaniu",
                                        FontSize = 13,
                                        TextColor = Color.FromArgb("#555555")
                                    }.Column(0).CenterVertical(),

                                    new Switch()
                                        .Column(1)
                                        .Bind(Switch.IsToggledProperty, nameof(WebAppEditViewModel.IsCustomJsEnabled))
                                }
                            },

                            new Label
                            {
                                Text = "Gotowe szablony (kliknij, aby wstawić kod):",
                                FontSize = 12,
                                TextColor = Colors.Gray
                            },

                            BuildPresetsRow(viewModel.JsPresets, nameof(WebAppEditViewModel.ApplyJsPresetCommand)),

                            new Editor
                            {
                                Placeholder = "// Wpisz własny kod JS... np. console.log('Witaj!');",
                                HeightRequest = 120,
                                FontFamily = "Consolas, monospace",
                                FontSize = 13,
                                BackgroundColor = Color.FromArgb("#F9F9FB")
                            }.Bind(Editor.TextProperty, nameof(WebAppEditViewModel.CustomJs))
                        }
                    }),

                    // SEKCJA 4: Reguły CSS/JS dla podstron lub fragmentów URL
                    BuildCardSection("Reguły CSS / JS dla podstron (URL Fragment)", new VerticalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            new Label
                            {
                                Text = "Możesz zdefiniować reguły uruchamiane tylko wtedy, gdy bieżący adres URL podstrony zawiera wskazany fragment (np. /watch dla wideo na YouTube, /settings, issues itp.).",
                                FontSize = 13,
                                TextColor = Color.FromArgb("#555555")
                            },

                            // Lista zdefiniowanych reguł
                            new CollectionView
                            {
                                ItemTemplate = new DataTemplate(BuildUrlRuleItemTemplate)
                            }.Bind(CollectionView.ItemsSourceProperty, nameof(WebAppEditViewModel.UrlRules)),

                            // Przycisk dodawania nowej reguły
                            new Button
                            {
                                Text = "+ Dodaj regułę dla podstrony",
                                FontSize = 13,
                                CornerRadius = 8,
                                BackgroundColor = Color.FromArgb("#EEF3FD"),
                                TextColor = Color.FromArgb("#1A73E8"),
                                HeightRequest = 38
                            }
                            .BindCommand(nameof(WebAppEditViewModel.ShowAddUrlRuleCommand))
                            .Bind(
                                Button.IsVisibleProperty,
                                nameof(WebAppEditViewModel.IsAddingUrlRule),
                                convert: (bool isAdding) => !isAdding
                            ),

                            // Formularz nowej reguły
                            BuildAddUrlRuleBox()
                                .Bind(Border.IsVisibleProperty, nameof(WebAppEditViewModel.IsAddingUrlRule))
                        }
                    }),

                    // SEKCJA 5: Zaawansowane (User Agent)
                    BuildCardSection("Opcje zaawansowane", new VerticalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            BuildFormField("Niestandardowy User-Agent (pozostaw puste dla domyślnego)",
                                new Entry { Placeholder = "np. Mozilla/5.0 (Windows NT 10.0; Win64; x64)..." }
                                    .Bind(Entry.TextProperty, nameof(WebAppEditViewModel.UserAgent)))
                        }
                    }),

                    // Przyciski zapisu i anulowania
                    new Grid
                    {
                        ColumnDefinitions = Columns.Define(Star, Star),
                        ColumnSpacing = 12,
                        Margin = new Thickness(0, 8, 0, 24),
                        Children =
                        {
                            new Button
                            {
                                Text = "Anuluj",
                                FontSize = 15,
                                CornerRadius = 8,
                                BackgroundColor = Color.FromArgb("#EEEEEE"),
                                TextColor = Color.FromArgb("#444444"),
                                HeightRequest = 46
                            }
                            .Column(0)
                            .BindCommand(nameof(WebAppEditViewModel.CancelCommand)),

                            new Button
                            {
                                Text = "Zapisz stronę",
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 15,
                                CornerRadius = 8,
                                BackgroundColor = AppColors.Primary,
                                TextColor = Colors.White,
                                HeightRequest = 46
                            }
                            .Column(1)
                            .BindCommand(nameof(WebAppEditViewModel.SaveCommand))
                        }
                    }
                }
            }
        };
    }

    private static View BuildAddUrlRuleBox()
    {
        return new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
            Stroke = Color.FromArgb("#1A73E8"),
            BackgroundColor = Color.FromArgb("#F9FBFF"),
            Padding = new Thickness(14),
            Margin = new Thickness(0, 8),
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        Text = "Nowa reguła dla podstrony",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 14,
                        TextColor = Color.FromArgb("#1A73E8")
                    },

                    BuildFormField("Nazwa reguły (np. Odtwarzacz wideo, Podstrona logowania):",
                        new Entry { Placeholder = "np. Widok filmu" }
                            .Bind(Entry.TextProperty, nameof(WebAppEditViewModel.NewRuleName))),

                    BuildFormField("Fragment lub wzorzec URL (np. /watch, settings, issues): *",
                        new Entry { Placeholder = "np. /watch" }
                            .Bind(Entry.TextProperty, nameof(WebAppEditViewModel.NewRuleUrlPattern))),

                    BuildFormField("Niestandardowy CSS dla tego fragmentu URL:",
                        new Editor
                        {
                            Placeholder = "/* CSS dla tej podstrony */",
                            HeightRequest = 80,
                            FontFamily = "Consolas, monospace",
                            FontSize = 12
                        }.Bind(Editor.TextProperty, nameof(WebAppEditViewModel.NewRuleCustomCss))),

                    BuildFormField("Niestandardowy JavaScript dla tego fragmentu URL:",
                        new Editor
                        {
                            Placeholder = "// JavaScript dla tej podstrony",
                            HeightRequest = 80,
                            FontFamily = "Consolas, monospace",
                            FontSize = 12
                        }.Bind(Editor.TextProperty, nameof(WebAppEditViewModel.NewRuleCustomJs))),

                    new Grid
                    {
                        ColumnDefinitions = Columns.Define(Star, Star),
                        ColumnSpacing = 8,
                        Children =
                        {
                            new Button
                            {
                                Text = "Anuluj",
                                FontSize = 13,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#EEEEEE"),
                                TextColor = Color.FromArgb("#555555"),
                                HeightRequest = 38
                            }
                            .Column(0)
                            .BindCommand(nameof(WebAppEditViewModel.CancelAddUrlRuleCommand)),

                            new Button
                            {
                                Text = "Dodaj tę regułę",
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 13,
                                CornerRadius = 6,
                                BackgroundColor = Color.FromArgb("#1A73E8"),
                                TextColor = Colors.White,
                                HeightRequest = 38
                            }
                            .Column(1)
                            .BindCommand(nameof(WebAppEditViewModel.SaveUrlRuleCommand))
                        }
                    }
                }
            }
        };
    }

    private static View BuildUrlRuleItemTemplate()
    {
        return new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
            Stroke = Color.FromArgb("#E0E0E0"),
            BackgroundColor = Color.FromArgb("#FAFAFA"),
            Padding = new Thickness(12, 10),
            Margin = new Thickness(0, 4),
            Content = new Grid
            {
                ColumnDefinitions = Columns.Define(Star, Auto, Auto),
                ColumnSpacing = 8,
                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 2,
                        Children =
                        {
                            new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 14,
                                TextColor = Color.FromArgb("#222222")
                            }.Bind(Label.TextProperty, nameof(UrlScriptRule.Name)),

                            new HorizontalStackLayout
                            {
                                Spacing = 6,
                                Children =
                                {
                                    new Label
                                    {
                                        Text = "URL zawiera:",
                                        FontSize = 11,
                                        TextColor = Colors.Gray
                                    },
                                    new Border
                                    {
                                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(4) },
                                        Stroke = Colors.Transparent,
                                        BackgroundColor = Color.FromArgb("#E8F0FE"),
                                        Padding = new Thickness(6, 1),
                                        Content = new Label
                                        {
                                            FontSize = 11,
                                            FontFamily = "Consolas, monospace",
                                            TextColor = Color.FromArgb("#1A73E8")
                                        }.Bind(Label.TextProperty, nameof(UrlScriptRule.UrlPattern))
                                    }
                                }
                            }
                        }
                    }.Column(0).CenterVertical(),

                    new Switch()
                        .Column(1)
                        .CenterVertical()
                        .Bind(Switch.IsToggledProperty, nameof(UrlScriptRule.IsEnabled)),

                    new Button
                    {
                        Text = "🗑️",
                        FontSize = 13,
                        CornerRadius = 6,
                        BackgroundColor = Color.FromArgb("#FDE8E8"),
                        TextColor = Color.FromArgb("#C5221F"),
                        WidthRequest = 36,
                        HeightRequest = 36,
                        Padding = new Thickness(0)
                    }
                    .Column(2)
                    .CenterVertical()
                    .BindCommand(
                        nameof(WebAppEditViewModel.RemoveUrlRuleCommand),
                        source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(WebAppEditViewModel)),
                        parameterPath: "."
                    )
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

    private static View BuildFormField(string labelText, View input)
    {
        return new VerticalStackLayout
        {
            Spacing = 6,
            Children =
            {
                new Label
                {
                    Text = labelText,
                    FontSize = 13,
                    TextColor = Color.FromArgb("#444444")
                },
                input
            }
        };
    }

    private static View BuildPresetsRow(ScriptPreset[] presets, string commandName)
    {
        var layout = new HorizontalStackLayout
        {
            Spacing = 8
        };

        foreach (var preset in presets)
        {
            var btn = new Button
            {
                Text = preset.Name,
                FontSize = 11,
                CornerRadius = 6,
                BackgroundColor = Color.FromArgb("#F0F4FF"),
                TextColor = Color.FromArgb("#1A73E8"),
                Padding = new Thickness(10, 4),
                HeightRequest = 32
            };

            btn.BindCommand(
                commandName,
                source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(WebAppEditViewModel)),
                parameterPath: "."
            );
            btn.BindingContext = preset;

            layout.Children.Add(btn);
        }

        return new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            Content = layout
        };
    }
}
