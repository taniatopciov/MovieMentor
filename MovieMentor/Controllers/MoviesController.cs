using MovieMentor.DTO;

namespace MovieMentor.Controllers;

public static class MoviesController
{
    private static List<MovieDto> movieDtos = new()
    {
        new MovieDto(1, "Batman Begins"),
        new MovieDto(2, "Encanto"),
    };

    public static IEnumerable<MovieDto> GetMovies()
    {
        return movieDtos;
    }

    public static MovieDto? GetMovie(int id)
    {
        return movieDtos.FirstOrDefault(m => m.Id == id);
    }

    public static IEnumerable<Tag> GetTags()
    {
        return new List<Tag>
        {
            new("Multiple", "Genre", new List<string> { "Action", "Drama", "Comedy", "Thriller", "Horror", "Ceva", "Altceva", "Miau", "Bau", "Ceau", "Milu", "Ham", "Cuc", "Pam", "Dam", "Mumu", "Zuzu", "Gugu" }),
            new("Single", "Director", new List<string> { "Christopher Nolan", "Steven Spielberg" }),
            new("Multiple", "Actors", new List<string> { "Adam Sandler", "Zendaya" }),
            new("Single", "Duration",
                new List<string> { "short (< 90 min)", "medium (90 min - 120 min)", "long (> 120 min)" ,  "Ceva", "Altceva", "Miau", "Bau", "Ceau", "Milu", "Ham", "Cuc", "Pam", "Dam", "Mumu", "Zuzu", "Gugu"}),
            new("Single", "Year", new List<string> { "'80s", "'90s", "2000s", "2010s", "2020s" }),
            new("Single", "Rating", new List<string> { "> 9", "8-9", "7-8", "6-7", "< 6" }),
            new("Multiple", "Awards",
                new List<string> { "Best Picture", "Best Director", "Best Actor", "Best Screenplay" }),
            new("Single", "Country", new List<string> { "USA", "Romania", "Germany", "France", "Spain" }),
        };
    }
}
