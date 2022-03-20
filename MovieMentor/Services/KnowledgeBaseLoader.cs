using Microsoft.EntityFrameworkCore;
using MovieMentor.DAL;
using MovieMentor.Data;
using MovieMentor.DTO;
using MovieMentor.Rules;
using MovieMentorCore.Models;
using static MovieMentor.Rules.Rules;
using ValueType = MovieMentor.Data.ValueType;

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

    private static readonly PredicateRule DurationPredicateRule =
        new PredicateRule.Builder("DurationType", "long (> 120 min)")
            .AddChoice("short (< 90 min)", v => v < 90)
            .AddChoice("medium (90 min - 120 min)", v => v is >= 90 and < 120)
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
            new(nameof(ValueType.Single), YearChoice, new List<string> { "'80s", "'90s", "2000s", "2010s", "2020s" }),
            new(nameof(ValueType.Single), RatingChoice, new List<string> { "> 9", "8-9", "7-8", "6-7", "< 6" }),
            // new(nameof(ValueType.Multiple), AwardsChoice, _movieContext.Awards.Select(a => a.Name).ToList()),
            new(nameof(ValueType.Single), CountryChoice, _movieContext.Countries.Select(c => c.Name).ToList()),
        };
    }

    public IDictionary<string, IList<RuleDefinition>> GetRules()
    {
        var durationRange = new List<RuleDefinition>();

        var moviesDefinitions = _movieContext.Movies
            .Include(movie => movie.Actors).ThenInclude(a => a.Country)
            .Include(nameof(Movie.Awards))
            .Include(nameof(Movie.Country))
            .Include(nameof(Movie.Directors))
            .Include(nameof(Movie.Genres))
            .Select(m => MovieDefinition(m.ID.ToString(), m.Title,
                m.Year.ToString(), m.Rating,
                AddDurationRange(durationRange, m.Duration).ToString(), m.Country.Name))
            .Cast<RuleDefinition>()
            .ToList();


        var genreDefinition = _movieContext.Genres
            .Select(g => GenreDefinition(g.Name))
            .Cast<RuleDefinition>()
            .ToList();

        var countryDefinition = _movieContext.Countries
            .Select(c => CountryDefinition(c.Name))
            .Cast<RuleDefinition>()
            .ToList();

        return new Dictionary<string, IList<RuleDefinition>>
        {
            {
                "Movie", moviesDefinitions
            },
            {
                "Genre", genreDefinition
            },
            {
                "Country", countryDefinition
            },
            {
                "DurationRange", durationRange
            },
            {
                "SearchMovie", new List<RuleDefinition>
                {
                    SearchMovieDefinition(),
                }
            }
        };
    }

    private static int AddDurationRange(ICollection<RuleDefinition> definitions, int value)
    {
        var label = DurationPredicateRule.Evaluate(value);
        definitions.Add(DurationRangeDefinition(label, value.ToString()));

        return value;
    }
}
