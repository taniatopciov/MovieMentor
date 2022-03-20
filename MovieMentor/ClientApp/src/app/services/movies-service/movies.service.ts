import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import Tag from "./tag";
import ResponseChoices from "../../home/responseChoices";

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

  getRecommendation(selectedOptions: ResponseChoices[]) {
    // var obj = {
    //   choices: selectedOptions
    // }
    //
    // console.log(obj);

    return this.http.post(this.inferenceUrl, {
      choices: selectedOptions
    });
  }
}
