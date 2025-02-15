using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PocClientSync.Services;


namespace PocClientSync.ViewModel
{
    public partial class SyncViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        public SyncViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        public async Task PullAllAsync()
        {
            await _apiService.PullServerAsync(true);

            await Toast.Make("Baixado todos os dados").Show();
        }

        [RelayCommand]
        public async Task PullNowAsync()
        {
            await _apiService.PullServerAsync(false);
            await Toast.Make("Baixado dados agora").Show();
        }

        [RelayCommand]
        public async Task PushAsync()
        {
           var time =  await _apiService.PushServerAsync();
            
            await Toast.Make("Dados enviado para servidor").Show();

            var docsErros = await _apiService.PushDocsToServerAsync(time);

            if (docsErros.Any())
            {
                await Toast.Make("Alguns documentos não foram enviados").Show();
            }
            else
            {
                await Toast.Make("Documentos enviados para servidor").Show();
            }
        }
    }
}