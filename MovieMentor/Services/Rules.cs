using MovieMentorCore.Models;

namespace MovieMentor.Services;

public static class Rules
{
    public const string DurationRangeRule = "DurationRange";
    public const string YearRangeRule = "YearRange";
    public const string RatingRangeRule = "RatingRange";
    public const string MovieRule = "Movie";
    public const string SearchMovieRule = "SearchMovie";
    public const string GenrePredicateRule = "GenrePredicate";

    private static class Parameters
    {
        public const string ID = "ID";
        public const string Title = "Title";
        public const string Country = "Country";
        public const string Directors = "Directors";
        public const string Genres = "Genres";
        public const string Actors = "Actors";
        public const string Year = "Year";
        public const string Rating = "Rating";
        public const string Duration = "Duration";
        public const string Name = "Name";
        public const string Value = "Value";
    }

    #region Duration Range

    public static RuleDefinition.Instance DurationRangeDefinition(string name, string value) =>
        DurationRangeInstance(new Parameter.SingleValue(name), new Parameter.SingleValue(value));

    public static RuleDefinition.Instance DurationRangeInstance(Parameter name, Parameter value) => new(
        DurationRangeRule,
        new ParameterList.Builder()
            .AddParameter(Parameters.Name, name)
            .AddParameter(Parameters.Value, value)
            .Build());

    #endregion


    #region Year Range

    public static RuleDefinition.Instance YearRangeDefinition(string name, string value) =>
        YearRangeInstance(new Parameter.SingleValue(name), new Parameter.SingleValue(value));

    public static RuleDefinition.Instance YearRangeInstance(Parameter name, Parameter value) => new(YearRangeRule,
        new ParameterList.Builder()
            .AddParameter(Parameters.Name, name)
            .AddParameter(Parameters.Value, value)
            .Build());

    #endregion

    #region Movie

    public static RuleDefinition.Instance MovieDefinition(string id, string title, string year, string rating,
        string duration, string country, ISet<string> actors, ISet<string> directors,
        ISet<string> genres) =>
        MovieRuleInstance(new Parameter.SingleValue(id),
            new Parameter.SingleValue(title),
            new Parameter.SingleValue(year),
            new Parameter.SingleValue(rating),
            new Parameter.SingleValue(duration),
            new Parameter.SingleValue(country),
            new Parameter.MultipleValues(actors),
            new Parameter.MultipleValues(directors),
            new Parameter.MultipleValues(genres));

    public static RuleDefinition.Instance MovieRuleInstance(Parameter id, Parameter title, Parameter year,
        Parameter rating, Parameter duration, Parameter country, Parameter actors, Parameter directors,
        Parameter genres) => new(MovieRule, new ParameterList.Builder()
        .AddParameter(Parameters.ID, id)
        .AddParameter(Parameters.Title, title)
        .AddParameter(Parameters.Year, year)
        .AddParameter(Parameters.Rating, rating)
        .AddParameter(Parameters.Duration, duration)
        .AddParameter(Parameters.Country, country)
        .AddParameter(Parameters.Actors, actors)
        .AddParameter(Parameters.Directors, directors)
        .AddParameter(Parameters.Genres, genres)
        .Build());

    #endregion

    #region Rating Range

    public static RuleDefinition.Instance RatingRangeDefinition(string name, string value) =>
        RatingRangeInstance(new Parameter.SingleValue(name), new Parameter.SingleValue(value));

    public static RuleDefinition.Instance RatingRangeInstance(Parameter name, Parameter value) => new(RatingRangeRule,
        new ParameterList.Builder()
            .AddParameter(Parameters.Name, name)
            .AddParameter(Parameters.Value, value)
            .Build());

    #endregion

    #region Search Movie

    public static RuleDefinition.Composite SearchMovieDefinition()
    {
        const int idIndex = 0;
        const int yearIndex = 1;
        const int yearRangeIndex = 2;
        const int ratingIndex = 3;
        const int ratingRangeIndex = 4;
        const int durationTypeIndex = 5;
        const int durationIndex = 6;
        const int countryIndex = 7;
        const int actorsIndex = 8;
        const int directorsIndex = 9;
        const int genresIndex = 10;
        const int genresPredicateIndex = 11;

        return new RuleDefinition.Composite(SearchMovieInstance(
            new Parameter.Reference(idIndex),
            new Parameter.Reference(yearRangeIndex),
            new Parameter.Reference(ratingRangeIndex),
            new Parameter.Reference(durationTypeIndex),
            new Parameter.Reference(countryIndex),
            new Parameter.Reference(actorsIndex),
            new Parameter.Reference(directorsIndex),
            new Parameter.Reference(genresPredicateIndex)
        ), new List<RuleDefinition.Instance>
        {
            MovieRuleInstance(
                new Parameter.Reference(idIndex),
                new Parameter.DontCare(),
                new Parameter.Reference(yearIndex),
                new Parameter.Reference(ratingIndex),
                new Parameter.Reference(durationIndex),
                new Parameter.Reference(countryIndex),
                new Parameter.Reference(actorsIndex),
                new Parameter.Reference(directorsIndex),
                new Parameter.Reference(genresIndex)
            ),
            DurationRangeInstance(
                new Parameter.Reference(durationTypeIndex),
                new Parameter.Reference(durationIndex)),
            YearRangeInstance(
                new Parameter.Reference(yearRangeIndex),
                new Parameter.Reference(yearIndex)
            ),
            GenrePredicateInstance(
                new Parameter.Reference(genresPredicateIndex),
                new Parameter.Reference(genresIndex)
                ),
            RatingRangeInstance(
                new Parameter.Reference(ratingRangeIndex),
                new Parameter.Reference(ratingIndex)),
        });
    }

    public static RuleDefinition.Instance SearchMovieInstance(Parameter id, Parameter yearRange, Parameter ratingRange,
        Parameter durationRange, Parameter country, Parameter actors, Parameter directors,
        Parameter genres) => new(SearchMovieRule, new ParameterList.Builder()
        .AddParameter(Parameters.ID, id)
        .AddParameter(YearRangeRule, yearRange)
        .AddParameter(RatingRangeRule, ratingRange)
        .AddParameter(DurationRangeRule, durationRange)
        .AddParameter(Parameters.Country, country)
        .AddParameter(Parameters.Actors, actors)
        .AddParameter(Parameters.Directors, directors)
        .AddParameter(GenrePredicateRule, genres)
        .Build());

    #endregion
    #region Genre Definition
    public static RuleDefinition.Instance GenrePredicateDefinition(string name, HashSet<string> values) =>
        GenrePredicateInstance(new Parameter.SingleValue(name), new Parameter.MultipleValues(values));

    public static RuleDefinition.Instance GenrePredicateInstance(Parameter name, Parameter value) => new(GenrePredicateRule,
        new ParameterList.Builder()
            .AddParameter(Parameters.Name, name)
            .AddParameter(Parameters.Value, value)
            .Build());
    #endregion
}