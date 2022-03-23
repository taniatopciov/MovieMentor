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
            .DistinctBy(p => p["ID"])
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
        var yearParameter = ExtractSingleValueParameter(recommendationDto, KnowledgeBaseLoader.YearChoice);
        var ratingParameter = ExtractSingleValueParameter(recommendationDto, KnowledgeBaseLoader.RatingChoice);
        var durationTypeParameter = ExtractSingleValueParameter(recommendationDto, KnowledgeBaseLoader.DurationChoice);
        var countryParameter = ExtractSingleValueParameter(recommendationDto, KnowledgeBaseLoader.CountryChoice);

        var actorsParameter = ExtractMultipleValuesParameter(recommendationDto, KnowledgeBaseLoader.ActorsChoice);
        var directorsParameter = ExtractMultipleValuesParameter(recommendationDto, KnowledgeBaseLoader.DirectorChoice);
        var awardsParameter = ExtractMultipleValuesParameter(recommendationDto, KnowledgeBaseLoader.AwardsChoice);
        var genresParameter = ExtractMultipleValuesParameter(recommendationDto, KnowledgeBaseLoader.GenreChoice);

        return Rules.SearchMovieInstance(
            new Parameter.Reference(0),
            yearParameter,
            ratingParameter,
            durationTypeParameter,
            countryParameter,
            actorsParameter,
            directorsParameter,
            awardsParameter,
            genresParameter);
    }

    private static Parameter ExtractSingleValueParameter(RecommendationDto recommendationDto, string fieldName)
    {
        var value = recommendationDto.GetSingleValue(fieldName);

        return value == null ? new Parameter.DontCare() : new Parameter.SingleValue(value);
    }

    private static Parameter ExtractMultipleValuesParameter(RecommendationDto recommendationDto, string fieldName)
    {
        var values = recommendationDto.GetValues(fieldName);

        return values == null ? new Parameter.DontCare() : new Parameter.MultipleValues(values.ToHashSet());
    }
}
