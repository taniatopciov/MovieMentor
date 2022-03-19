using MovieMentor.Data;

namespace MovieMentor.DAL;

public static class DbInitializer
{
    public static void Initialize(MovieContext context)
    {
        context.Database.EnsureCreated();

        if (context.Movies.Any())
        {
            return;
        }

        var countries = new List<Country>
        {
            new() { Name = "Romania" },
            new() { Name = "USA" },
            new() { Name = "United Kingdom" },
        };
        foreach (var country in countries)
        {
            context.Countries.Add(country);
        }

        context.SaveChanges();

        // var movies = new List<Movie>
        // {
        //     new Movie
        //     {
        //     }
        // };
        //
        // foreach (var movie in movies)
        // {
        //     context.Movies.Add(movie);
        // }
        //
        // context.SaveChanges();
        //
        // var director = new List<Director>
        // {
        //     new() { Name = "Christopher Nolan" },
        //     new() { Name = "Jon Watts" },
        //     new() { Name = "Jared Bush" },
        //     new() { Name = "Byron Howard" },
        //     new() { Name = "Charise Castro Smith" },
        // };
    }
}
