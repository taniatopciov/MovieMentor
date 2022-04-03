using CsvHelper.Configuration;

namespace MovieMentorDatabase;

public class MovieMap : ClassMap<MovieCsv>
{
    public MovieMap()
    {
        Map(m => m.Name).Name("name");
        Map(m => m.Genre).Name("genre");
        Map(m => m.Year).Name("year");
        Map(m => m.Score).Name("score");
        Map(m => m.Director).Name("director");
        Map(m => m.Actor).Name("actor");
        Map(m => m.Country).Name("country");
        Map(m => m.Duration).Name("runtime");
        Map(m => m.Link).Name("link");
    }
}
