import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import Tag from "./tag";
import ResponseChoices from "../../home/responseChoices";
import Movie from "../../home/movie";

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
    return this.http.post<Movie[]>(this.inferenceUrl, {
      choices: selectedOptions
    });
  }
}
