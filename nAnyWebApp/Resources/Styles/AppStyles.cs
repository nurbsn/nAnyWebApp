namespace nAnyWebApp.Resources.Styles;

public static class AppStyles
{
	// Helper to access resources
	public static object? Get(string resourceName)
	{
		if (App.Current?.Resources.TryGetValue(resourceName, out var resource) ?? false)
		{
			return resource;
		}

		return null;
	}

	// The following (named) styles are defined in XAML in the default template
	// Implicit styles are defined in XAML in Resources/Styles/Styles.xaml
	public static Style Headline { get; } = new Style<Label>()
		.AddAppThemeBinding(Label.TextColorProperty, AppColors.MidnightBlue, AppColors.White)
		.Add(Label.FontSizeProperty, 32)
		.Add(Label.HorizontalOptionsProperty, LayoutOptions.Center)
		.Add(Label.HorizontalTextAlignmentProperty, TextAlignment.Center);

	public static Style SubHeadline { get; } = new Style<Label>()
		.AddAppThemeBinding(Label.TextColorProperty, AppColors.MidnightBlue, AppColors.White)
		.Add(Label.FontSizeProperty, 24)
		.Add(Label.HorizontalOptionsProperty, LayoutOptions.Center)
		.Add(Label.HorizontalTextAlignmentProperty, TextAlignment.Center);
}
