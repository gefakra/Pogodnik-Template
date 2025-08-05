namespace WeatherApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("notespage", typeof(NotesPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("calendarpage", typeof(CalendarPage));
        }
    }
}
