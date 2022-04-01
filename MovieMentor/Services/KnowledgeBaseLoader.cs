using Microsoft.EntityFrameworkCore;
using MovieMentor.DAL;
using MovieMentor.DTO;
using MovieMentorCore.Data;
using MovieMentorCore.Models;
using ValueType = MovieMentorCore.Data.ValueType;

namespace MovieMentor.Services;

public class KnowledgeBaseLoader : IKnowledgeBaseLoader
{
    public const string GenreChoice = "Watching Intention";
    public const string DirectorChoice = "Director";
    public const string ActorsChoice = "Actors";
    public const string DurationChoice = "Duration";
    public const string YearChoice = "Year";
    public const string RatingChoice = "IMDb Rating";
    public const string CountryChoice = "Country";

    private static readonly PredicateRule<string> GenrePredicateRule =
        new PredicateRule<string>.Builder(Rules.GenrePredicateRule, "Relax and feel good (Comedy, Romance, Animation)")
            .AddChoice("Let your imagination run wild (Fantasy, Sci-fi)",
                genre => genre.Equals("Fantasy", StringComparison.OrdinalIgnoreCase) || genre.Equals("Sci-fi", StringComparison.OrdinalIgnoreCase))
            .AddChoice("Get ready for suspense and anxiety (Horror, Crime, Thriller, Mystery)",
                genre => genre.Equals("Horror", StringComparison.OrdinalIgnoreCase) || genre.Equals("Crime", StringComparison.OrdinalIgnoreCase) || genre.Equals("Thriller", StringComparison.OrdinalIgnoreCase) || genre.Equals("Mystery", StringComparison.OrdinalIgnoreCase))
            .AddChoice("Keep in touch with real life (Biography, Documentary)",
                genre => genre.Equals("Biography", StringComparison.OrdinalIgnoreCase) || genre.Equals("Documentary", StringComparison.OrdinalIgnoreCase))
            .AddChoice("Try to follow every thread of the story (Drama, Adventure, Action)",
                genre => genre.Equals("Drama", StringComparison.OrdinalIgnoreCase) || genre.Equals("Adventure", StringComparison.OrdinalIgnoreCase) || genre.Equals("Action", StringComparison.OrdinalIgnoreCase) || genre.Equals("Western", StringComparison.OrdinalIgnoreCase))
            .Build();

    private static readonly PredicateRule<int> DurationPredicateRule =
        new PredicateRule<int>.Builder(Rules.DurationRangeRule, "long (> 120 min)")
            .AddChoice("short (< 90 min)", v => v < 90)
            .AddChoice("medium (90 min - 120 min)", v => v is >= 90 and < 120)
            .Build();

    private static readonly PredicateRule<int> YearPredicateRule =
        new PredicateRule<int>.Builder(Rules.YearRangeRule, "2020s")
            .AddChoice("Released before the 80s", y => y is < 1980)
            .AddChoice("80s",y => y is >= 1980 and < 1990 )
            .AddChoice("90s", y => y is >= 1990 and < 2000)
            .AddChoice("2000s", y => y is >= 2000 and < 2010)
            .AddChoice("2010s", y => y is >= 2010 and < 2020)
            .Build();

    private static readonly PredicateRule<double> RatingPredicateRule =
        new PredicateRule<double>.Builder(Rules.RatingRangeRule, "< 5")
            .AddChoice("> 9", r => r >= 9.0)
            .AddChoice("7-9", r => r is >= 7.0 and < 9.0)
            .AddChoice("5-7", r => r is >= 5.0 and < 7.0)
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
            new(nameof(ValueType.Multiple), GenreChoice, false, GenrePredicateRule.GetLabels()),
            new(nameof(ValueType.Multiple), DirectorChoice, true, _movieContext.Directors.Select(d => d.Name).ToList()),
            new(nameof(ValueType.Multiple), ActorsChoice, true, _movieContext.Actors.Select(a => a.Name).ToList()),
            new(nameof(ValueType.Single), DurationChoice, true, DurationPredicateRule.GetLabels()),
            new(nameof(ValueType.Single), YearChoice, false, YearPredicateRule.GetLabels()),
            new(nameof(ValueType.Single), RatingChoice, true, RatingPredicateRule.GetLabels()),
            new(nameof(ValueType.Single), CountryChoice, true, _movieContext.Countries.Select(c => c.Name).ToList()),
        };
    }

    public IDictionary<string, IList<RuleDefinition>> GetRules()
    {
        var durationRange = new List<RuleDefinition>();
        var yearRange = new List<RuleDefinition>();
        var ratingRange = new List<RuleDefinition>();
        var genres = new List<RuleDefinition>();

        var moviesDefinitions = _movieContext.Movies
            .Include(movie => movie.Actors)
            .Include(nameof(Movie.Country))
            .Include(nameof(Movie.Directors))
            .Include(nameof(Movie.Genres))
            .Select(m => Rules.MovieDefinition(
                m.ID.ToString(),
                m.Title,
                AddYearRange(yearRange, m.Year).ToString(),
                AddRatingRange(ratingRange, m.Rating),
                AddDurationRange(durationRange, m.Duration).ToString(),
                m.Country.Name,
                m.Actors.Select(a => a.Name).ToHashSet(),
                m.Directors.Select(d => d.Name).ToHashSet(),
                m.Genres.Select(g => AddGenrePredicate(genres, g.Name)).ToHashSet()))
            .Cast<RuleDefinition>()
            .ToList();

        return new Dictionary<string, IList<RuleDefinition>>
        {
            {
                Rules.MovieRule, moviesDefinitions
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
            },
            {
                GenrePredicateRule.Name, genres
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

    private static string AddGenrePredicate(ICollection<RuleDefinition> definitions, string value)
    {
        if (definitions.Any(d => d.ParametersList[value] != null))
        {
            return value;
        }

        var label = GenrePredicateRule.Evaluate(value);
        definitions.Add(Rules.GenrePredicateDefinition(label, new HashSet<string>{value}));

        return value;
    }
}