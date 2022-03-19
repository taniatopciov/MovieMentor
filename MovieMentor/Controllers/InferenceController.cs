using Microsoft.AspNetCore.Mvc;
using MovieMentor.DTO;
using MovieMentor.Services;
using MovieMentorCore.Models;

namespace MovieMentor.Controllers;

public static class InferenceController
{
    public static IEnumerable<MovieDto> Infer([FromBody] RecommendationDto recommendationDto,
        [FromServices] IInferenceMachineService inferenceMachineService, [FromServices] IMoviesService moviesService)
    {
        var ruleInstance = Convert(recommendationDto);

        var possibilities = inferenceMachineService.Infer(ruleInstance);

        return possibilities
            .Where(p => p.Length >= 1)
            .Select(p => !int.TryParse(p[0], out var id) ? null : moviesService.GetMovie(id))
            .OfType<MovieDto>();
    }

    private static RuleInstance Convert(RecommendationDto recommendationDto)
    {
        return new RuleInstance("SearchMovie", new List<Parameter>
        {
            new Parameter.Reference(0), // id
            new Parameter.Reference(1), // title
            new Parameter.Concrete("2021"), // year
            new Parameter.Concrete("7.3"), // rating
            new Parameter.Reference(2), // duration
            new Parameter.Concrete("medium (90 min - 120 min)"), // duration type (long
        });
    }
}
