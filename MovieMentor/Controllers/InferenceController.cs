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
            .Where(p => p.Count >= 1)
            .Select(parameters =>
            {
                var idParameter = parameters["ID"];
                if (idParameter is Parameter.SingleValue (var value))
                {
                    return !int.TryParse(value, out var id) ? null : moviesService.GetMovie(id);
                }

                return null;
            })
            .OfType<MovieDto>();
    }

    private static RuleDefinition.Instance Convert(RecommendationDto recommendationDto)
    {
        return Rules.Rules.SearchMovieInstance(
            new Parameter.Reference(0),
            new Parameter.Reference(1),
            new Parameter.SingleValue("2021"),
            new Parameter.SingleValue("7.3"),
            new Parameter.Reference(2),
            new Parameter.SingleValue("medium (90 min - 120 min)"));
    }
}
