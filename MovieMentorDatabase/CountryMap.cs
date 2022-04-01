using CsvHelper.Configuration;
using MovieMentorCore.Data;

namespace MovieMentorDatabase;

public class CountryMap : ClassMap<Country>
{
    public CountryMap()
    {
        Map(m => m.Name).Name("country");
    }
}