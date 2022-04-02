using System.Globalization;
using System.Web;
using MovieMentorCore.Data;
using MovieMentorDatabase.DAL;
using CsvHelper;
using Newtonsoft.Json;

namespace MovieMentorDatabase;

public static class DbInitializer
{
    public static async Task Initialize(MovieContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (context.Movies.Any())
        {
            return;
        }

        Dictionary<string, Genre> genreDict = new();
        Dictionary<string, Actor> actorDict = new();
        Dictionary<string, Director> directorDict = new();
        Dictionary<string, Country> countryDict = new();
        HttpClient client = new HttpClient();

        using (var streamReader = new StreamReader(@"MoviesData\SEMovies.csv"))
        {
            using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
            {
                csvReader.Context.RegisterClassMap<MovieMap>();
                var records = csvReader.GetRecords<MovieCsv>().ToList();

                foreach (var record in records)
                {
                    if (!genreDict.TryGetValue(record.Genre, out var genre))
                    {
                        genre = new Genre {Name = record.Genre};
                        genreDict.Add(record.Genre, genre);
                    }

                    if (!actorDict.TryGetValue(record.Actor, out var actor))
                    {
                        actor = new Actor {Name = record.Actor};
                        actorDict.Add(record.Actor, actor);
                    }

                    if (!directorDict.TryGetValue(record.Director, out var director))
                    {
                        director = new Director {Name = record.Director};
                        directorDict.Add(record.Director, director);
                    }

                    if (!countryDict.TryGetValue(record.Country, out var country))
                    {
                        country = new Country {Name = record.Country};
                        countryDict.Add(record.Country, country);
                    }

                    var link = record.Link.Remove(record.Link.Length - 1, 1);
                    var movieId = link.Split('/').Last();
                    
                    var response = await client.GetAsync("https://imdb-api.com/en/API/Title/k_7pclxom2/" + movieId);
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    
                    var imageLink = (string)data.image ?? "";
                    var description = (string)data.plot ?? "";
                    
                    
                    
                    var movie = new Movie
                    {
                        Title = record.Name,
                        Actors = new List<Actor> {actor},
                        Country = country,
                        Directors = new List<Director> {director},
                        Duration = record.Duration,
                        Year = record.Year,
                        Genres = new List<Genre> {genre},
                        Rating = record.Score.ToString(),
                        Link = record.Link,
                        ImageLink = imageLink,
                        Description = description
                    };

                    context.Movies.Add(movie);
                }

                await context.SaveChangesAsync();
            }
        }

    }
}