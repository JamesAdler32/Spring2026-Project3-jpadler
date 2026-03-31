using System.ComponentModel.DataAnnotations;

namespace Spring2026_Project3_jpadler.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = default!;

        public string? IMDBLink { get; set; }

        public string? Genre { get; set; }

        public int ReleaseYear { get; set; }

        public byte[]? Poster { get; set; }
    }
}
