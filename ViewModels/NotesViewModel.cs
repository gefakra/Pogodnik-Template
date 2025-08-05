using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.ViewModels;

public class NotesViewModel : INotifyPropertyChanged
{
    private readonly INoteService _noteService;
    private readonly IWeatherService _weatherService;

    private string _city = string.Empty;
    private DateTime _selectedDate = DateTime.Today;
    private string _title = string.Empty;
    private string _noteText = string.Empty;
    private Note _selectedNote;

    public ObservableCollection<Note> Notes { get; } = new();

    public bool IsCreateMode => Mode == "create";
    public bool IsListMode => Mode == "list";

    public ICommand SaveNoteCommand { get; }
    public ICommand DeleteNoteCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    public NotesViewModel(INoteService noteService, IWeatherService weatherService)
    {
        _noteService = noteService ?? throw new ArgumentNullException(nameof(noteService));
        _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));

        SaveNoteCommand = new Command(async () => await SaveNoteAsync(), () => CanSaveNote);
        DeleteNoteCommand = new Command(async () => await DeleteNoteAsync(), () => SelectedNote != null);
    }
    private string _mode;
    public string Mode
    {
        get => _mode;
        set
        {
            if (_mode != value)
            {
                _mode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCreateMode));
                OnPropertyChanged(nameof(IsListMode));

                if (_mode == "create")
                {
                    Title = "";
                    NoteText = "";
                    RaiseCanExecuteChanged();
                }
                
                if (_mode == "list")
                    _ = LoadNotesAsync();
            }
        }
    }
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
                _ = LoadNotesAsync();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value))
                RaiseCanExecuteChanged();
        }
    }

    public string NoteText
    {
        get => _noteText;
        set
        {
            if (SetProperty(ref _noteText, value))
                RaiseCanExecuteChanged();
        }
    }

    public string City
    {
        get => _city;
        set
        {
            if (SetProperty(ref _city, value))
            {
                _ = LoadNotesAsync();
                RaiseCanExecuteChanged();
            }
        }
    }

    public Note SelectedNote
    {
        get => _selectedNote;
        set
        {
            if (SetProperty(ref _selectedNote, value))
            {
                RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(SelectedNote));
            }

        }
    }

    public void SetCity(string city)
    {
        City = city?.Trim();
    }

    private bool CanSaveNote =>
        !string.IsNullOrWhiteSpace(Title)
        && !string.IsNullOrWhiteSpace(NoteText)
        && !string.IsNullOrWhiteSpace(City);

    private async Task LoadNotesAsync()
    {        
        var allNotes = await _noteService.GetNotesAsync();

        var filtered = string.IsNullOrWhiteSpace(City)
            ? allNotes.Where(n => n.CreatedAt.Date == SelectedDate.Date)
            : allNotes.Where(n => n.CreatedAt.Date == SelectedDate.Date
                                 && n.Location.Equals(City, StringComparison.OrdinalIgnoreCase));

        Notes.Clear();
        foreach (var note in filtered.OrderByDescending(n => n.CreatedAt))
            Notes.Add(note);

        SelectedNote = Notes.Any() ? Notes.First() : null;
    }

    private async Task SaveNoteAsync()
    {
        if (!CanSaveNote) return;

        var weather = await _weatherService.GetWeatherAsync(City);

        var note = new Note
        {
            Title = Title,
            NoteText = NoteText,
            CreatedAt = SelectedDate,
            Location = City,
            WeatherCondition = weather.Description,
            IsActive = true
        };

        await _noteService.SaveNoteAsync(note);
        await LoadNotesAsync();
        Title = string.Empty;
        NoteText = string.Empty;
        RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(NoteText));
    }

    private async Task DeleteNoteAsync()
    {
        if (SelectedNote == null) return;

        await _noteService.DeleteNoteAsync(SelectedNote);
        await LoadNotesAsync();
        SelectedNote = null;
    }

    private void RaiseCanExecuteChanged()
    {
        (SaveNoteCommand as Command)?.ChangeCanExecute();
        (DeleteNoteCommand as Command)?.ChangeCanExecute();
    }

    protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value)) return false;

        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
