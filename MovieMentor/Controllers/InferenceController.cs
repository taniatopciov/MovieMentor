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
        var year = recommendationDto.GetSingleValue(KnowledgeBaseLoader.YearChoice);
        Parameter yearParameter = year == null ? new Parameter.DontCare() : new Parameter.SingleValue(year);

        var rating = recommendationDto.GetSingleValue(KnowledgeBaseLoader.RatingChoice);
        Parameter ratingParameter = rating == null ? new Parameter.DontCare() : new Parameter.SingleValue(rating);

        var durationType = recommendationDto.GetSingleValue(KnowledgeBaseLoader.DurationChoice);
        Parameter durationTypeParameter =
            durationType == null ? new Parameter.DontCare() : new Parameter.SingleValue(durationType);

        var country = recommendationDto.GetSingleValue(KnowledgeBaseLoader.CountryChoice);
        Parameter countryParameter = country == null ? new Parameter.DontCare() : new Parameter.SingleValue(country);

        return Rules.Rules.SearchMovieInstance(
            new Parameter.Reference(0),
            yearParameter,
            ratingParameter,
            durationTypeParameter,
            countryParameter);
    }
}
