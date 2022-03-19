using Microsoft.AspNetCore.Mvc;
using MovieMentor.DTO;
using MovieMentor.Services;

namespace MovieMentor.Controllers;

public static class MoviesController
{
    public static IEnumerable<MovieDto> GetMovies([FromServices] IMoviesService moviesService)
    {
        return moviesService.GetAllMovies();
    }

    public static MovieDto? GetMovie(int id, [FromServices] IMoviesService moviesService)
    {
        return moviesService.GetMovie(id);
    }

    public static IEnumerable<ChoiceDto> GetTags([FromServices] IKnowledgeBaseLoader knowledgeBaseLoader)
    {
        return knowledgeBaseLoader.GetChoices();
        // return movieContext.Tags
        //     .Where(tag => tag.SourceType == SourceType.Database)
        //     .Select(tag => ConvertTag(tag, movieContext));
    }

    // private static TagDto ConvertTag(Tag tag, MovieContext movieContext)
    // {
    //     var values = new List<string>();
    //
    //     return new TagDto(tag.Name, tag.ValueType.ToString(), values);
    // }
}
