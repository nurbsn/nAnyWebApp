namespace nAnyWebApp;

public partial class App : Application
{
	readonly AppShell shell;

	public App(AppShell appShell)
	{
		// TODO: Please give feedback on the inclusion of styles defined in XAML at https://github.com/mrlacey/MauiAppAccelerator/issues/10
		InitializeComponent();

		shell = appShell;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(shell);
	}
}
