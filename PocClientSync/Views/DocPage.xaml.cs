using CommunityToolkit.Maui.Alerts;
using PocClientSync.ViewModel;

namespace PocClientSync.Views;

public partial class DocPage : ContentPage
{
    private readonly DocPageViewModel _viewModel;

    public DocPage(DocPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.GetDocs();
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        var status = await CheckAndRequestLocationPermissionAsync();

        if (status != PermissionStatus.Granted)
        {
            await Shell.Current.DisplayAlert("Error", "No Permission", "OK");
        }

        if (!MediaPicker.Default.IsCaptureSupported)
        {
            return;
        }

        FileResult? file = null;
#if WINDOWS

        file = await FilePicker.Default.PickAsync(new PickOptions()
        {
            PickerTitle = "Pick images",
            FileTypes = FilePickerFileType.Png
        });
#else
        file = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions() { Title = "Pick images" });
#endif

        if (file == null)
        {
            await Shell.Current.DisplayAlert("Error", "No file", "OK");

            return;
        }

        // save the file into local storage
        string localFilePath = Path.Combine(FileSystem.CacheDirectory, file.FileName);

        await using Stream sourceStream = await file.OpenReadAsync();
        await using FileStream localFileStream = File.OpenWrite(localFilePath);

        await sourceStream.CopyToAsync(localFileStream);

        try
        {
            await _viewModel.SaveAsync(localFilePath, file.FileName, file.FileName);
           Toast.Make("Upload file");
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Error", "No Save File", "OK");
        }
    }

    private async Task<PermissionStatus> CheckAndRequestLocationPermissionAsync()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // Prompt the user to turn on in settings
            // On iOS once a permission has been denied it may not be requested again from the application
            return status;
        }

        if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
        {
            // Prompt the user with additional information as to why the permission is needed
        }

        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        return status;
    }
}