using Microsoft.EntityFrameworkCore;
using MovieMentor.Data;

namespace MovieMentor.DAL;

public class MovieContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    public DbSet<Country> Countries { get; set; }

    public MovieContext(DbContextOptions<MovieContext> options) : base(options)
    {
    }
}
