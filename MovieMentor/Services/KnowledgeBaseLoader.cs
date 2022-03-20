using Microsoft.EntityFrameworkCore;
using MovieMentor.DAL;
using MovieMentor.Data;
using MovieMentor.DTO;
using MovieMentorCore.Models;
using static MovieMentor.Rules.Rules;
using Range = MovieMentorCore.Utils.Range;
using ValueType = MovieMentor.Data.ValueType;

namespace MovieMentor.Services;

public class KnowledgeBaseLoader : IKnowledgeBaseLoader
{
    private readonly List<string> _durations = new()
    {
        "short (< 90 min)", "medium (90 min - 120 min)", "long (> 120 min)"
    };

    private readonly MovieContext _movieContext;

    public KnowledgeBaseLoader(MovieContext movieContext)
    {
        _movieContext = movieContext;
    }

    public IEnumerable<ChoiceDto> GetChoices()
    {
        return new List<ChoiceDto>
        {
            new(nameof(ValueType.Multiple), "Genre", _movieContext.Genres.Select(g => g.Name).ToList()),
            new(nameof(ValueType.Multiple), "Director", _movieContext.Directors.Select(d => d.Name).ToList()),
            new(nameof(ValueType.Multiple), "Actors", _movieContext.Actors.Select(a => a.Name).ToList()),
            new(nameof(ValueType.Single), "Duration", _durations),
            new(nameof(ValueType.Single), "Year", new List<string> { "'80s", "'90s", "2000s", "2010s", "2020s" }),
            new(nameof(ValueType.Single), "Rating", new List<string> { "> 9", "8-9", "7-8", "6-7", "< 6" }),
            new(nameof(ValueType.Multiple), "Awards", _movieContext.Awards.Select(a => a.Name).ToList()),
            new(nameof(ValueType.Single), "Country", _movieContext.Countries.Select(c => c.Name).ToList()),
        };
    }

    public IDictionary<string, IList<RuleDefinition>> GetRules()
    {
        // var durationRange = new List<RuleDefinition>();


        // todo combine with movie year
        var durationRange = new Range(90)
            .Select(value => DurationRangeDefinition(_durations[0], value.ToString()))
            .Concat(new Range(90, 120).Select(value =>
                DurationRangeDefinition(_durations[1], value.ToString())))
            .Concat(new Range(120, 300).Select(value =>
                DurationRangeDefinition(_durations[2], value.ToString())))
            .Cast<RuleDefinition>()
            .ToList();

        var moviesDefinitions = _movieContext.Movies
            .Include(movie => movie.Actors).ThenInclude(a => a.Country)
            .Include(nameof(Movie.Awards))
            .Include(nameof(Movie.Country))
            .Include(nameof(Movie.Directors))
            .Include(nameof(Movie.Genres))
            .Select(m => MovieDefinition(m.ID.ToString(), m.Title,
                m.Year.ToString(), m.Rating, m.Duration.ToString()))
            .Cast<RuleDefinition>()
            .ToList();

        // var moviesDefinitions = _movieContext.Movies
        //     .Include(movie => movie.Actors).ThenInclude(a => a.Country)
        //     .Include(nameof(Movie.Awards))
        //     .Include(nameof(Movie.Country))
        //     .Include(nameof(Movie.Directors))
        //     .Include(nameof(Movie.Genres))
        //     .Select(m => new RuleDefinition.Composite("Movie", new List<Parameter>
        //     {
        //         new Parameter.Concrete(m.ID.ToString()),
        //         new Parameter.Concrete(m.Title),
        //         new Parameter.Concrete(m.Year.ToString()),
        //         new Parameter.Concrete(m.Rating),
        //         new Parameter.Concrete(m.Duration.ToString()),
        //     }, new List<RuleInstance>
        //     {
        //         AddToList(durationRange, new RuleInstance("DurationRange", new List<Parameter>
        //         {
        //             new Parameter.Concrete(m.Duration < 90 ? _durations[0] :
        //                 m.Duration < 120 ? _durations[1] : _durations[2]),
        //             new Parameter.Concrete(m.Duration.ToString()),
        //         }))
        //     }))
        //     .Cast<RuleDefinition>()
        //     .ToList();

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

    private static RuleDefinition.Instance AddToList(ICollection<RuleDefinition> definitions,
        RuleDefinition.Instance ruleInstance)
    {
        definitions.Add(new RuleDefinition.Instance(ruleInstance.Name, ruleInstance.ParametersList));

        return ruleInstance;
    }
}
