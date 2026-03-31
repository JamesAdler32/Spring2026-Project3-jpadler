using System.ComponentModel.DataAnnotations;

namespace Spring2026_Project3_jpadler.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = default!;
        
        public string? Gender { get; set; }
        
        public int Age { get; set; }

        public string? IMDBLink { get; set; }
        
        public byte[]? Photo { get; set; }
    }
}
