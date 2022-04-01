using CsvHelper.Configuration;
using MovieMentorCore.Data;

namespace MovieMentorDatabase;

public class ActorMap : ClassMap<Actor>
{
    public ActorMap()
    {
        Map(m => m.Name).Name("actor");
    }
}