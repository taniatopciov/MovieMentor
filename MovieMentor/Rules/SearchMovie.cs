﻿using MovieMentorCore.Models;

namespace MovieMentor.Rules;

public static partial class Rules
{
    public static RuleDefinition.Composite SearchMovieDefinition()
    {
        const int idIndex = 0;
        const int yearIndex = 1;
        const int ratingIndex = 2;
        const int durationTypeIndex = 3;
        const int durationIndex = 4;
        
        return new RuleDefinition.Composite("SearchMovie",
            new ParameterList.Builder()
                .AddParameter("ID", new Parameter.Reference(idIndex))
                .AddParameter("Year", new Parameter.Reference(yearIndex))
                .AddParameter("Rating", new Parameter.Reference(ratingIndex))
                .AddParameter("DurationType", new Parameter.Reference(durationTypeIndex))
                .Build()
            , new List<RuleDefinition.Instance>
            {
                MovieRuleInstance(
                    new Parameter.Reference(idIndex),
                    new Parameter.DontCare(),
                    new Parameter.Reference(yearIndex),
                    new Parameter.Reference(ratingIndex),
                    new Parameter.Reference(durationIndex)),
                DurationRangeInstance(
                    new Parameter.Reference(durationTypeIndex),
                    new Parameter.Reference(durationIndex)),
            });
    }

    public static RuleDefinition.Instance SearchMovieInstance(Parameter id, Parameter year,
        Parameter rating, Parameter durationType) => new("SearchMovie",
        new ParameterList.Builder()
            .AddParameter("ID", id)
            .AddParameter("Year", year)
            .AddParameter("Rating", rating)
            .AddParameter("DurationType", durationType)
            .Build());
}
