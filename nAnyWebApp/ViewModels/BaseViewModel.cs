using CommunityToolkit.Mvvm.ComponentModel;

namespace nAnyWebApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private bool isBusy;
}
