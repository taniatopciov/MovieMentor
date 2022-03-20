using MovieMentorCore.Data;
using MovieMentorDatabase.DAL;

namespace MovieMentorDatabase;

public static class DbInitializer
{
    public static async Task Initialize(MovieContext context)
    {
        // await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (context.Movies.Any())
        {
            return;
        }

        var romania = new Country { Name = "Romania" };
        var usa = new Country { Name = "USA" };
        var uk = new Country { Name = "United Kingdom" };
        var argentina = new Country { Name = "Argentina" };
        var columbia = new Country { Name = "Columbia" };
        var countries = new List<Country>
        {
            romania,
            usa,
            uk,
            argentina,
            columbia,
        };
        foreach (var country in countries)
        {
            context.Countries.Add(country);
        }

        await context.SaveChangesAsync();


        var action = new Genre { Name = "Action" };
        var adventure = new Genre { Name = "Adventure" };
        var thriller = new Genre { Name = "Thriller" };
        var comedy = new Genre { Name = "Comedy" };
        var animated = new Genre { Name = "Animated" };
        var genres = new List<Genre>
        {
            action,
            adventure,
            thriller,
            comedy,
            animated
        };
        foreach (var genre in genres)
        {
            context.Genres.Add(genre);
        }

        await context.SaveChangesAsync();

        var nolan = new Director { Name = "Christopher Nolan" };
        var watts = new Director { Name = "Jon Watts" };
        var bush = new Director { Name = "Jared Bush" };
        var howard = new Director { Name = "Byron Howard" };
        var smith = new Director { Name = "Charise Castro Smith" };
        var directors = new List<Director>
        {
            nolan,
            watts,
            bush,
            howard,
            smith,
        };
        foreach (var director in directors)
        {
            context.Directors.Add(director);
        }

        await context.SaveChangesAsync();

        var bestCinematography = new Award { Name = "Best Cinematography" };
        var bestAnimatedFilm = new Award { Name = "Best Animated Film" };
        var awards = new List<Award>
        {
            bestCinematography,
            bestAnimatedFilm,
        };
        foreach (var award in awards)
        {
            context.Awards.Add(award);
        }

        await context.SaveChangesAsync();

        var bale = new Actor { Name = "Christian Bale", Country = uk };
        var freeman = new Actor { Name = "Morgan Freeman", Country = usa };
        var maluma = new Actor { Name = "Maluma", Country = columbia };
        var beatriz = new Actor { Name = "Stephanie Beatriz", Country = argentina };
        var bota = new Actor { Name = "Cristian Bota", Country = romania };
        var papadopol = new Actor { Name = "Alexandru Papadopol", Country = romania };
        var actors = new List<Actor>
        {
            bale,
            freeman,
            maluma,
            beatriz,
            bota,
            papadopol
        };
        foreach (var actor in actors)
        {
            context.Actors.Add(actor);
        }

        await context.SaveChangesAsync();


        var movies = new List<Movie>
        {
            new()
            {
                Title = "Batman Begins",
                Actors = new List<Actor> { bale, freeman },
                Awards = new List<Award> { bestCinematography },
                Country = usa,
                Directors = new List<Director> { nolan },
                Duration = 140,
                Rating = "8.3",
                Year = 2005,
                Genres = new List<Genre> { action, thriller },
            },
            new()
            {
                Title = "Encanto",
                Actors = new List<Actor> { beatriz, maluma },
                Awards = new List<Award> { bestAnimatedFilm },
                Country = usa,
                Directors = new List<Director> { howard, bush },
                Duration = 109,
                Rating = "7.3",
                Year = 2021,
                Genres = new List<Genre> { comedy, animated },
            },
            new()
            {
                Title = "Bani negri",
                Actors = new List<Actor> { bota, papadopol },
                Awards = new List<Award>(),
                Country = romania,
                Directors = new List<Director>(),
                Duration = 400,
                Rating = "8.1",
                Year = 2020,
                Genres = new List<Genre> { action, comedy },
            },
        };

        foreach (var movie in movies)
        {
            context.Movies.Add(movie);
        }

        await context.SaveChangesAsync();
    }
}
