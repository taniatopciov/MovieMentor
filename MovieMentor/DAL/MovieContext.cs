using Microsoft.EntityFrameworkCore;
using MovieMentorCore.Data;

namespace MovieMentor.DAL;

public class MovieContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Director> Directors { get; set; }

    public DbSet<Award> Awards { get; set; }

    public DbSet<Actor> Actors { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public MovieContext(DbContextOptions<MovieContext> options) : base(options)
    {
    }
}
