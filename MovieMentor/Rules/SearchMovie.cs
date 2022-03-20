using MovieMentorCore.Models;

namespace MovieMentor.Rules;

public static partial class Rules
{
    public static RuleDefinition.Composite SearchMovieDefinition()
    {
        const int idIndex = 0;
        const int yearIndex = 1;
        const int yearRangeIndex = 2;
        const int ratingIndex = 3;
        const int durationTypeIndex = 4;
        const int durationIndex = 5;
        const int countryIndex = 6;

        return new RuleDefinition.Composite("SearchMovie",
            new ParameterList.Builder()
                .AddParameter("ID", new Parameter.Reference(idIndex))
                .AddParameter("YearRange", new Parameter.Reference(yearRangeIndex))
                .AddParameter("Rating", new Parameter.Reference(ratingIndex))
                .AddParameter("DurationType", new Parameter.Reference(durationTypeIndex))
                .AddParameter("Country", new Parameter.Reference(countryIndex))
                .Build()
            , new List<RuleDefinition.Instance>
            {
                MovieRuleInstance(
                    new Parameter.Reference(idIndex),
                    new Parameter.DontCare(),
                    new Parameter.Reference(yearIndex),
                    new Parameter.Reference(ratingIndex),
                    new Parameter.Reference(durationIndex),
                    new Parameter.Reference(countryIndex)),
                DurationRangeInstance(
                    new Parameter.Reference(durationTypeIndex),
                    new Parameter.Reference(durationIndex)),
                YearRangeInstance(
                    new Parameter.Reference(yearRangeIndex),
                    new Parameter.Reference(yearIndex)
                ),
                CountryInstance(new Parameter.Reference(countryIndex))
            });
    }

    public static RuleDefinition.Instance SearchMovieInstance(Parameter id, Parameter year, Parameter rating,
        Parameter durationType, Parameter country) => new("SearchMovie",
        new ParameterList.Builder()
            .AddParameter("ID", id)
            .AddParameter("YearRange", year)
            .AddParameter("Rating", rating)
            .AddParameter("DurationType", durationType)
            .AddParameter("Country", country)
            .Build());
}
