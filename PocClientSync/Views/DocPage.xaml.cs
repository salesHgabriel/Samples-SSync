﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocClientSync.ViewModel;

namespace PocClientSync.Views;

public partial class DocPage : ContentPage
{
    private readonly DocPageViewModel _viewModel;
    public DocPage(DocPageViewModel viewModel)
    {
        BindingContext = _viewModel = viewModel;
        _viewModel = viewModel;
        InitializeComponent();
        
        
    }
    
    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        var status = await CheckAndRequestLocationPermissionAsync();

        if (status != PermissionStatus.Granted)
        {
            await Shell.Current.DisplayAlert("Error", "No Permission", "OK");
        }
        
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions(){Title = "Photos"});

            if (photo == null)
            {
                await Shell.Current.DisplayAlert("Error", "No photo", "OK");

                return;
            } 
            
            // save the file into local storage
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

            await using Stream sourceStream = await photo.OpenReadAsync();
            await using FileStream localFileStream = File.OpenWrite(localFilePath);
                
            
            await sourceStream.CopyToAsync(localFileStream);
            
           await _viewModel.SaveAsync(localFilePath, photo.FileName, photo.FileName);
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