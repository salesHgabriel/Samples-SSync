using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PocClientSync.Models;
using PocClientSync.Repositories;
using SSync.Client.LitebDB.Enums;

namespace PocClientSync.ViewModel;

public partial class DocPageViewModel : ObservableObject
{
    private readonly IDocRepo _docRepo;

    public DocPageViewModel(IDocRepo docRepo)
    {
        _docRepo = docRepo;
    }

    [ObservableProperty]
    public ObservableCollection<Doc>? docs;
    
    [RelayCommand]
    public void GetDocs()
    {
        var docs = _docRepo.GetDocs();

        Docs = new ObservableCollection<Doc>(docs);
    }

    
    public async Task SaveAsync(string path, string fileName, string name)
    {
        var doc = new Doc(Guid.NewGuid(), Time.UTC)
        {
            Name = name,
            Path = path,
            FileName = fileName
        };

       await _docRepo.SaveAsync(doc);
    }
}