using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PocClientSync.Models;
using PocClientSync.Repositories;
using SSync.Client.SQLite.Enums;
using System.Collections.ObjectModel;


namespace PocClientSync.ViewModel
{
    public partial class NotePageViewModel : ObservableObject
    {
        private readonly IUserRepo _userRepo;

        private readonly INoteRepo _noteRepo;

        public NotePageViewModel(IUserRepo userRepo, INoteRepo noteRepo)
        {
            _userRepo = userRepo;
            _noteRepo = noteRepo;

     
        }


        [ObservableProperty]
        public ObservableCollection<User>? users;

        [ObservableProperty]
        public ObservableCollection<Note>? notes;

        [ObservableProperty]
        public User? userSelected;

        [ObservableProperty]
        public bool completed;

        [ObservableProperty]
        public string? message;

        public async Task GetUsersAsync()
        {
            var users = await _userRepo.GetUsers();

            Users = new ObservableCollection<User>(users);
        }

        public async Task GetNotesAsync()
        {
            var notes = await _noteRepo.GetNotes();

            Notes = new ObservableCollection<Note>(notes);
        }

        [RelayCommand]
        public async Task GetList()
        {
           await GetNotesAsync();
            await GetUsersAsync();
        }

        [RelayCommand]
        public void SelecteUser(User userSelected)
        {
            UserSelected = userSelected;
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            var entity = new Note(Guid.NewGuid(), Time.UTC)
            {
                Completed = Completed,
                Message = Message,
                UserId = UserSelected!.Id,
                UserName = UserSelected.Name
            };

            await _noteRepo.Save(entity);

            await Shell.Current.DisplayAlert("Alert", "You have been alerted", "OK");

            CleanFields();
            await GetNotesAsync();
        }

        [RelayCommand]
        public async Task UpdateAsync(Note vm)
        {
            var noteUp = await _noteRepo.GetNoteBydId(vm.Id);

            noteUp.Message = "Updatedd";
            noteUp.Completed = !noteUp.Completed;

            await _noteRepo.Update(noteUp);

            await GetNotesAsync();
        }

        [RelayCommand]
        public async Task DeleteAsync(Note vm)
        {
            var note =  await _noteRepo.GetNoteBydId(vm.Id);

            await _noteRepo.Delete(note);

            await GetNotesAsync();
        }

        [RelayCommand]
        public Task DropAsync()
        {
            _userRepo.Drop();

            return Task.CompletedTask;
        }

        public void CleanFields()
        {
            UserSelected = null;
            Message = string.Empty;
        }
    }
}