using Microsoft.EntityFrameworkCore;
using SQLite;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class NoteService : INoteService
    {        
        private readonly NotesDbContext _context;

        public NoteService(NotesDbContext _Context)
        {
            _context = _Context;
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            return await _context.Notes.ToListAsync();
        }

        public async Task<Note> GetNoteAsync(int id)
        {
            return await _context.Notes.FindAsync(id);
        }

        public async Task<int> SaveNoteAsync(Note note)
        {
            if (note.Id != 0)
                _context.Notes.Update(note);
            else
                await _context.Notes.AddAsync(note);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteNoteAsync(Note note)
        {
            _context.Notes.Remove(note);
            return await _context.SaveChangesAsync();
        }

    }
}