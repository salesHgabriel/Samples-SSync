using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PocClientSync.Repositories;
using PocClientSync.Services;
using PocClientSync.ViewModel;
using PocClientSync.Views;

namespace PocClientSync
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif




            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<INoteRepo, NoteRepo>();
            builder.Services.AddScoped<IDocRepo, DocRepo>();
            builder.Services.AddScoped<ISyncRepository, SyncRepository>();

            builder.Services.AddScoped<IApiService, ApiService>();

            builder.Services.AddScoped<UserPage>();
            builder.Services.AddScoped<UserPageViewModel>();

            builder.Services.AddScoped<NotePage>();
            builder.Services.AddScoped<NotePageViewModel>();


            builder.Services.AddScoped<MainPage>();
            builder.Services.AddScoped<SyncViewModel>();
            
            builder.Services.AddScoped<DocPage>();
            builder.Services.AddScoped<DocPageViewModel>();







            return builder.Build();
        }
    }
}
