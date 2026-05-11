namespace nAnyWebApp.Views;

public partial class ustawieniaPage : ContentPage
{
	public ustawieniaPage(ustawieniaViewModel viewModel)
	{
		BindingContext = viewModel;

		Content = new CollectionView
		{
			
		};
	}
}
