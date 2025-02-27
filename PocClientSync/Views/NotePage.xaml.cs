using PocClientSync.ViewModel;

namespace PocClientSync.Views;

public partial class NotePage : ContentPage
{
    NotePageViewModel _vm;
    public NotePage(NotePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }


    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var t1 =  _vm.GetNotesAsync();
        var t2 =  _vm.GetUsersAsync();

       await Task.WhenAll(t1, t2);
    }

}