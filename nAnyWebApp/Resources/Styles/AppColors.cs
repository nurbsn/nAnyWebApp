namespace nAnyWebApp.Resources.Styles;

// These are the colors and brushes used in the default template
// They are also in /Resources/Styles/Colors.xaml
public class AppColors : ResourceDictionary
{
	public static Color Primary { get; } = Color.FromArgb("#512BD4");
	public static Color PrimaryDark { get; } = Color.FromArgb("#ac99ea");
	public static Color PrimaryDarkText { get; } = Color.FromArgb("#242424");
	public static Color Secondary { get; } = Color.FromArgb("#DFD8F7");
	public static Color SecondaryDarkText { get; } = Color.FromArgb("#9880e5");
	public static Color Tertiary { get; } = Color.FromArgb("#2B0B98");

	public static Color White { get; } = Colors.White;
	public static Color Black { get; } = Colors.Black;
	public static Color Magenta { get; } = Color.FromArgb("#D600AA");
	public static Color MidnightBlue { get; } = Color.FromArgb("#190649");
	public static Color OffBlack { get; } = Color.FromArgb("#1f1f1f");

	public static Color Gray100 { get; } = Color.FromArgb("#E1E1E1");
	public static Color Gray200 { get; } = Color.FromArgb("#C8C8C8");
	public static Color Gray300 { get; } = Color.FromArgb("#ACACAC");
	public static Color Gray400 { get; } = Color.FromArgb("#919191");
	public static Color Gray500 { get; } = Color.FromArgb("#6E6E6E");
	public static Color Gray600 { get; } = Color.FromArgb("#404040");
	public static Color Gray900 { get; } = Color.FromArgb("#212121");
	public static Color Gray950 { get; } = Color.FromArgb("#141414");

	public static SolidColorBrush PrimaryBrush { get; } = new(Primary);
	public static SolidColorBrush SecondaryBrush { get; } = new(Secondary);
	public static SolidColorBrush TertiaryBrush { get; } = new(Tertiary);
	public static SolidColorBrush WhiteBrush { get; } = new(White);
	public static SolidColorBrush BlackBrush { get; } = new(Black);

	public static SolidColorBrush Gray100Brush { get; } = new(Gray100);
	public static SolidColorBrush Gray200Brush { get; } = new(Gray200);
	public static SolidColorBrush Gray300Brush { get; } = new(Gray300);
	public static SolidColorBrush Gray400Brush { get; } = new(Gray400);
	public static SolidColorBrush Gray500Brush { get; } = new(Gray500);
	public static SolidColorBrush Gray600Brush { get; } = new(Gray600);
	public static SolidColorBrush Gray900Brush { get; } = new(Gray900);
	public static SolidColorBrush Gray950Brush { get; } = new(Gray950);
}
