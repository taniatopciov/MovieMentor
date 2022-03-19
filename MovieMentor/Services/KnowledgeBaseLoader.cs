using Microsoft.EntityFrameworkCore;
using MovieMentor.DAL;
using MovieMentor.Data;
using MovieMentor.DTO;
using MovieMentorCore.Models;
using Parameter = MovieMentorCore.Models.Parameter;
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
        var moviesDefinitions = _movieContext.Movies
            .Include(movie => movie.Actors).ThenInclude(a => a.Country)
            .Include(nameof(Movie.Awards))
            .Include(nameof(Movie.Country))
            .Include(nameof(Movie.Directors))
            .Include(nameof(Movie.Genres))
            .Select(m => new RuleDefinition.Concrete("Movie", new List<Parameter.Concrete>
            {
                new(m.ID.ToString()),
                new(m.Title),
                new(m.Year.ToString()),
                new(m.Rating),
                new(m.Duration.ToString()),
            }))
            .Cast<RuleDefinition>().ToList();

        var genreDefinition = _movieContext.Genres
            .Select(g => new RuleDefinition.Concrete("Genre", new List<Parameter.Concrete>
            {
                new(g.Name)
            }))
            .Cast<RuleDefinition>().ToList();

        var countryDefinition = _movieContext.Countries
            .Select(c => new RuleDefinition.Concrete("Country", new List<Parameter.Concrete>
            {
                new(c.Name)
            }))
            .Cast<RuleDefinition>().ToList();

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
            // {
            //     "Duration", _durations.Select(d => new RuleDefinition.Concrete("Duration")).ToList()
            // }
        };
    }
}
