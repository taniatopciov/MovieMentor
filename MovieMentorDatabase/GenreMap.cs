using CsvHelper.Configuration;
using MovieMentorCore.Data;

namespace MovieMentorDatabase;

public class GenreMap : ClassMap<Genre>
{
    public GenreMap()
    {
        Map(m => m.Name).Name("genre");
    }
}