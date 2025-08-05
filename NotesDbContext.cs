using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp
{
    public class NotesDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }

        private readonly string _dbPath;

        public NotesDbContext(DbContextOptions<NotesDbContext> options)
            : base(options)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = System.IO.Path.Combine(folder, "notes.db");            
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite($"Filename={_dbPath}");
        }
    }
}
