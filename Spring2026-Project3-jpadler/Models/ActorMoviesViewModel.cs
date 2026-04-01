using Spring2026_Project3_jpadler.Controllers;

namespace Spring2026_Project3_jpadler.Models
{
    public class ActorMoviesViewModel
    {
        public required Actor Actor;

        public required IEnumerable<Movie?> ActorMovies;

        public required ReviewBundle? Reviews;
    }
}