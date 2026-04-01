using Spring2026_Project3_jpadler.Controllers;

namespace Spring2026_Project3_jpadler.Models
{
    public class MovieActorsViewModel
    {
        public required Movie Movie;

        public required IEnumerable<Actor?> MovieActors;

        public required ReviewBundle? Reviews;
    }
}
