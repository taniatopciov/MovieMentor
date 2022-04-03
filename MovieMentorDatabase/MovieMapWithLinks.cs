using CsvHelper.Configuration;

namespace MovieMentorDatabase;

public class MovieMapWithLinks : ClassMap<MovieCsvWithLinks>
{
    public MovieMapWithLinks()
    {
        Map(m => m.Name).Name("Name");
        Map(m => m.Genre).Name("Genre");
        Map(m => m.Year).Name("Year");
        Map(m => m.Score).Name("Score");
        Map(m => m.Director).Name("Director");
        Map(m => m.Actor).Name("Actor");
        Map(m => m.Country).Name("Country");
        Map(m => m.Duration).Name("Duration");
        Map(m => m.Link).Name("Link");
        Map(m => m.ImageLink).Name("ImageLink");
        Map(m => m.Description).Name("Description");
    }
}
