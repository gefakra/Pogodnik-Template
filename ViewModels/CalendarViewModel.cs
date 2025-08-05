using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        readonly INoteService _notesService;
        readonly IStorageService _storageService;
        private DateTime _selectedDate;
        public DateTime SelectedDate {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                   _ = LoadNotesAsync();
                }
            }
        }
        public ObservableCollection<Note> Notes { get; set; } = new();

        public ICommand AddNoteCommand { get; }

        public string City { get; }

        public CalendarViewModel(INoteService ns, IStorageService storageService)
        {
            _notesService = ns;
            SelectedDate = DateTime.Today;
            _storageService = storageService;
            City = _storageService.GetLastCity();
            AddNoteCommand = new Command(async () => await Add());            
        }

        private async Task LoadNotesAsync()
        {
            Notes.Clear();
            var all = await _notesService.GetNotesAsync();
            foreach (var n in all.Where(x => x.CreatedAt.Date == SelectedDate.Date))
                Notes.Add(n);
        }

        async Task Add()
        {
            await Shell.Current.GoToAsync($"/notespage?mode=create&date={SelectedDate:yyyy-MM-dd}&city={Uri.EscapeDataString(City)}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

    }

}
