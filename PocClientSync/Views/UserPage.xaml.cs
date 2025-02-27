using PocClientSync.ViewModel;

namespace PocClientSync.Views;

public partial class UserPage : ContentPage
{
    private UserPageViewModel _vm;

    public UserPage(UserPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

       await _vm.GetUsersAsync();
    }

}