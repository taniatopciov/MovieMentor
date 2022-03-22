using MovieMentorCore.Models;

namespace MovieMentor.Services;

public static class Rules
{
    public const string CountryRule = "Country";
    public const string DurationRangeRule = "DurationRange";
    public const string GenreRule = "Genre";
    public const string YearRangeRule = "YearRange";
    public const string RatingRangeRule = "RatingRange";
    public const string MovieRule = "Movie";
    public const string SearchMovieRule = "SearchMovie";

    private static class Parameters
    {
        public const string ID = "ID";
        public const string Title = "Title";
        public const string Year = "Year";
        public const string Rating = "Rating";
        public const string Duration = "Duration";
        public const string Country = "Country";
        public const string Name = "Name";
        public const string Value = "Value";
    }

    #region Country

    public static RuleDefinition.Instance CountryDefinition(string name) =>
        CountryInstance(new Parameter.SingleValue(name));

    public static RuleDefinition.Instance CountryInstance(Parameter name) => new(CountryRule,
        new ParameterList.Builder()
            .AddParameter(Parameters.Name, name)
            .Build());

    #endregion

    #region Duration Range

    public static RuleDefinition.Instance DurationRangeDefinition(string name, string value) =>
        DurationRangeInstance(new Parameter.SingleValue(name), new Parameter.SingleValue(value));

    public static RuleDefinition.Instance DurationRangeInstance(Parameter name, Parameter value) => new(DurationRangeRule,
        new ParameterList.Builder()
            .AddParameter(Parameters.Name, name)
            .AddParameter(Parameters.Value, value)
            .Build());

    #endregion

    #region Genre

    public static RuleDefinition.Instance GenreDefinition(string name) =>
        GenreInstance(new Parameter.SingleValue(name));

    public static RuleDefinition.Instance GenreInstance(Parameter name) => new(GenreRule,
        new ParameterList.Builder()
            .AddParameter(Parameters.Name, name)
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
        string duration, string country) =>
        MovieRuleInstance(new Parameter.SingleValue(id),
            new Parameter.SingleValue(title),
            new Parameter.SingleValue(year),
            new Parameter.SingleValue(rating),
            new Parameter.SingleValue(duration),
            new Parameter.SingleValue(country));

    public static RuleDefinition.Instance MovieRuleInstance(Parameter id, Parameter title, Parameter year,
        Parameter rating, Parameter duration, Parameter country) =>
        new(MovieRule, new ParameterList.Builder()
            .AddParameter(Parameters.ID, id)
            .AddParameter(Parameters.Title, title)
            .AddParameter(Parameters.Year, year)
            .AddParameter(Parameters.Rating, rating)
            .AddParameter(Parameters.Duration, duration)
            .AddParameter(Parameters.Country, country)
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

        return new RuleDefinition.Composite(SearchMovieInstance(
            new Parameter.Reference(idIndex),
            new Parameter.Reference(yearRangeIndex),
            new Parameter.Reference(ratingRangeIndex),
            new Parameter.Reference(durationTypeIndex),
            new Parameter.Reference(countryIndex)
        ), new List<RuleDefinition.Instance>
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
            RatingRangeInstance(
                new Parameter.Reference(ratingRangeIndex),
                new Parameter.Reference(ratingIndex)),
            CountryInstance(new Parameter.Reference(countryIndex))
        });
    }

    public static RuleDefinition.Instance SearchMovieInstance(Parameter id, Parameter yearRange, Parameter ratingRange,
        Parameter durationRange, Parameter country) => new(SearchMovieRule,
        new ParameterList.Builder()
            .AddParameter(Parameters.ID, id)
            .AddParameter(YearRangeRule, yearRange)
            .AddParameter(RatingRangeRule, ratingRange)
            .AddParameter(DurationRangeRule, durationRange)
            .AddParameter(CountryRule, country)
            .Build());

    #endregion
}
