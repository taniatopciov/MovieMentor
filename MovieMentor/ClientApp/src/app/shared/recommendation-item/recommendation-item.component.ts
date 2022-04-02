import {Component, Input, OnInit} from '@angular/core';
import Movie from "../../home/movie";

@Component({
  selector: 'recommendation-item',
  templateUrl: './recommendation-item.component.html',
  styleUrls: ['./recommendation-item.component.css']
})
export class RecommendationItemComponent implements OnInit {

  @Input() movieData: Movie;

  constructor() {
    this.movieData = {} as Movie;
  }

  ngOnInit(): void {
  }

  getItemsString(list: string[]) {
    let items = "";
    list.forEach((item, index) => {
      items += item;
      if (index != list.length - 1) {
        items += ", ";
      }
    });
    return items;
  }
}
