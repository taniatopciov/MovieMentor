import {Inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import Tag from "./tag";
import {of} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MoviesService {

  private movieTagsUrl = '/movies/tags';
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  getTags() {
    // return this.http.get<Tag[]>(this.baseUrl + this.movieTagsUrl);
    return of<Tag[]>([{type: "Multiple", name: "Genre", values: ["Action", "Drama", "Comedy", "Thriller", "Horror"]},
      {type: "Single", name: "Director", values: ["Christopher Nolan", "Steven Spielberg"]},
      {type: "Multiple", name: "Actors", values: ["Adam Sandler", "Zendaya"]},
      {type: "Single", name: "Duration", values: ["short (< 90 min)", "medium (90 min - 120 min)", "long (> 120 min)"]},
      {type: "Single", name: "Year", values: ["'80s", "'90s", "2000s", "2010s", "2020s"]},
      {type: "Single", name: "Rating", values: ["> 9", "8-9", "7-8", "6-7", "< 6"]},
      {type: "Multiple", name: "Awards", values: ["Best Picture", "Best Director", "Best Actor", "Best Screenplay"]},
      {type: "Single", name: "Country", values: ["USA", "Romania", "Germany", "France", "Spain"]}]);
  }
}
