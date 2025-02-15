using PocClientSync.ViewModel;

namespace PocClientSync.Views;

public partial class NotePage : ContentPage
{
 
    public NotePage(NotePageViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }


}