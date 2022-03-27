import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import Tag from "./tag";
import ResponseChoices from "../../home/responseChoices";
import Movie from "../../home/movie";
import {map} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MoviesService {

  private movieTagsUrl = '/api/movies/tags';
  private inferenceUrl = '/api/inference';

  constructor(private http: HttpClient) {
  }

  getTags() {
    return this.http.get<Tag[]>(this.movieTagsUrl);
  }

  getRecommendations(selectedOptions: ResponseChoices[]) {
    return this.http.post<any[]>(this.inferenceUrl, {
      choices: selectedOptions
    }).pipe(map(data => {
      const movies: Movie[] = [];

      data.forEach(movie => {
        movies.push({
          ...movie,
          actors: movie.actors.map((actor: any) => actor.name),
          directors: movie.directors.map((director: any) => director.name)
        });
      });

      return movies;
    }));
  }
}
