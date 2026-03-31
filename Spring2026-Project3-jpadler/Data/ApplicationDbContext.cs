using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_jpadler.Models;

namespace Spring2026_Project3_jpadler.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Spring2026_Project3_jpadler.Models.Movie> Movie { get; set; } = default!;
        public DbSet<Spring2026_Project3_jpadler.Models.Actor> Actor { get; set; } = default!;
        public DbSet<Spring2026_Project3_jpadler.Models.MovieActor> MovieActor { get; set; } = default!;
    }
}
