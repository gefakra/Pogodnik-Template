using System;
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string NoteText { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        [Required]
        public string Location { get; set; }
        public string WeatherCondition { get; set; }
    }
}
