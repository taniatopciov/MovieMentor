using CsvHelper.Configuration;
using MovieMentorCore.Data;

namespace MovieMentorDatabase;

public class DirectorMap : ClassMap<Director>
{
    public DirectorMap()
    {
        Map(m => m.Name).Name("director");
    }
}