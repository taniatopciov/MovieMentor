using Microsoft.EntityFrameworkCore;
using MovieMentor.DAL;
using MovieMentor.DTO;
using MovieMentorCore.Data;
using MovieMentorCore.Models;
using ValueType = MovieMentorCore.Data.ValueType;

namespace MovieMentor.Services;

public class KnowledgeBaseLoader : IKnowledgeBaseLoader
{
    public const string GenreChoice = "Genre";
    public const string DirectorChoice = "Director";
    public const string ActorsChoice = "Actors";
    public const string DurationChoice = "Duration";
    public const string YearChoice = "Year";
    public const string RatingChoice = "Rating";
    public const string AwardsChoice = "Awards";
    public const string CountryChoice = "Country";

    private static readonly PredicateRule<int> DurationPredicateRule =
        new PredicateRule<int>.Builder(Rules.DurationRangeRule, "long (> 120 min)")
            .AddChoice("short (< 90 min)", v => v < 90)
            .AddChoice("medium (90 min - 120 min)", v => v is >= 90 and < 120)
            .Build();

    private static readonly PredicateRule<int> YearPredicateRule =
        new PredicateRule<int>.Builder(Rules.YearRangeRule, "Other")
            .AddChoice("'80", y => y is >= 1980 and < 1990)
            .AddChoice("'90", y => y is >= 1990 and < 2000)
            .AddChoice("2000s", y => y is >= 2000 and < 2010)
            .AddChoice("2010s", y => y is >= 2010 and < 2020)
            .AddChoice("2020s", y => y is >= 2020 and < 2030)
            .Build();

    private static readonly PredicateRule<double> RatingPredicateRule =
        new PredicateRule<double>.Builder(Rules.RatingRangeRule, "< 6")
            .AddChoice("> 9", r => r >= 9.0)
            .AddChoice("8-9", r => r is >= 8.0 and < 9.0)
            .AddChoice("7-8", r => r is >= 7.0 and < 8.0)
            .AddChoice("6-7", r => r is >= 6.0 and < 7.0)
            .Build();

    private readonly MovieContext _movieContext;

    public KnowledgeBaseLoader(MovieContext movieContext)
    {
        _movieContext = movieContext;
    }

    public IEnumerable<ChoiceDto> GetChoices()
    {
        return new List<ChoiceDto>
        {
            // new(nameof(ValueType.Multiple), GenreChoice, _movieContext.Genres.Select(g => g.Name).ToList()),
            // new(nameof(ValueType.Multiple), DirectorChoice, _movieContext.Directors.Select(d => d.Name).ToList()),
            // new(nameof(ValueType.Multiple), ActorsChoice, _movieContext.Actors.Select(a => a.Name).ToList()),
            new(nameof(ValueType.Single), DurationChoice, DurationPredicateRule.GetLabels()),
            new(nameof(ValueType.Single), YearChoice, YearPredicateRule.GetLabels()),
            new(nameof(ValueType.Single), RatingChoice, RatingPredicateRule.GetLabels()),
            // new(nameof(ValueType.Multiple), AwardsChoice, _movieContext.Awards.Select(a => a.Name).ToList()),
            new(nameof(ValueType.Single), CountryChoice, _movieContext.Countries.Select(c => c.Name).ToList()),
        };
    }

    public IDictionary<string, IList<RuleDefinition>> GetRules()
    {
        var durationRange = new List<RuleDefinition>();
        var yearRange = new List<RuleDefinition>();
        var ratingRange = new List<RuleDefinition>();

        var moviesDefinitions = _movieContext.Movies
            .Include(movie => movie.Actors).ThenInclude(a => a.Country)
            .Include(nameof(Movie.Awards))
            .Include(nameof(Movie.Country))
            .Include(nameof(Movie.Directors))
            .Include(nameof(Movie.Genres))
            .Select(m => Rules.MovieDefinition(m.ID.ToString(), m.Title,
                AddYearRange(yearRange, m.Year).ToString(), AddRatingRange(ratingRange, m.Rating),
                AddDurationRange(durationRange, m.Duration).ToString(), m.Country.Name))
            .Cast<RuleDefinition>()
            .ToList();


        var genreDefinition = _movieContext.Genres
            .Select(g => Rules.GenreDefinition(g.Name))
            .Cast<RuleDefinition>()
            .ToList();

        var countryDefinition = _movieContext.Countries
            .Select(c => Rules.CountryDefinition(c.Name))
            .Cast<RuleDefinition>()
            .ToList();

        return new Dictionary<string, IList<RuleDefinition>>
        {
            {
                Rules.MovieRule, moviesDefinitions
            },
            {
                Rules.GenreRule, genreDefinition
            },
            {
                Rules.CountryRule, countryDefinition
            },
            {
                DurationPredicateRule.Name, durationRange
            },
            {
                YearPredicateRule.Name, yearRange
            },
            {
                RatingPredicateRule.Name, ratingRange
            },
            {
                Rules.SearchMovieRule, new List<RuleDefinition>
                {
                    Rules.SearchMovieDefinition(),
                }
            }
        };
    }

    private static int AddDurationRange(ICollection<RuleDefinition> definitions, int value)
    {
        if (definitions.Any(d => d.ParametersList[value.ToString()] != null))
        {
            return value;
        }

        var label = DurationPredicateRule.Evaluate(value);
        definitions.Add(Rules.DurationRangeDefinition(label, value.ToString()));

        return value;
    }

    private static int AddYearRange(ICollection<RuleDefinition> definitions, int value)
    {
        if (definitions.Any(d => d.ParametersList[value.ToString()] != null))
        {
            return value;
        }

        var label = YearPredicateRule.Evaluate(value);
        definitions.Add(Rules.YearRangeDefinition(label, value.ToString()));

        return value;
    }

    private static string AddRatingRange(ICollection<RuleDefinition> definitions, string value)
    {
        if (definitions.Any(d => d.ParametersList[value] != null))
        {
            return value;
        }

        if (!double.TryParse(value, out var parsedValue))
        {
            return value;
        }

        var label = RatingPredicateRule.Evaluate(parsedValue);
        definitions.Add(Rules.RatingRangeDefinition(label, value));

        return value;
    }
}
