using PocClientSync.ViewModel;

namespace PocClientSync
{
    public partial class MainPage : ContentPage
    {

        private readonly SyncViewModel _vm;
        public MainPage(SyncViewModel vm)
        {
            BindingContext = vm;
            _vm = vm;
            InitializeComponent();
        }

    }

}
