using PocClientSync.ViewModel;

namespace PocClientSync.Views;

public partial class UserPage : ContentPage
{
	public UserPage(UserPageViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}