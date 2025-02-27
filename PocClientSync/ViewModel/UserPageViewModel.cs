using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PocClientSync.Models;
using PocClientSync.Repositories;
using SSync.Client.SQLite.Enums;
using System.Collections.ObjectModel;

namespace PocClientSync.ViewModel
{
    public partial class UserPageViewModel : ObservableObject
    {
        private readonly IUserRepo _userRepo;

        public UserPageViewModel(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [ObservableProperty]
        public ObservableCollection<User>? users;

        [ObservableProperty]
        public string? name;

        [ObservableProperty]
        public int age;


        [RelayCommand]
        public async Task GetUsersAsync()
        {
            var users = await _userRepo.GetUsers();

            Users = new ObservableCollection<User>(users);

        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            var user = new User(Guid.NewGuid(), Time.UTC)
            {
                Name = Name,
                Age = Age
            };

            await _userRepo.SaveAsync(user);

            await Shell.Current.DisplayAlert("Alert", "You have been alerted", "OK");

            CleanFields();
            await GetUsersAsync();
        }

        [RelayCommand]
        public async Task UpdateAsync(User vm)
        {
            var userUp = await _userRepo.GetBydId(vm.Id);


            userUp.Name = Name ?? "updated user";
            userUp.Age = Age == 0 ? new Random().Next(1, 20) : Age;

            await _userRepo.Update(userUp);

            await GetUsersAsync();


        }

        [RelayCommand]
        public async Task DeleteAsync(User vm)
        {
            var userUp = await _userRepo.GetBydId(vm.Id);

            await _userRepo.Delete(userUp);

            await GetUsersAsync();
        }

        [RelayCommand]
        public Task DropAsync()
        {
            _userRepo.Drop();

            return Task.CompletedTask;
        }


        public void CleanFields()
        {
            Name = string.Empty;

            Age = 0;
        }


        //TODO SYNC


    }
}