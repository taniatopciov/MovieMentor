import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {MoviesService} from "../services/movies-service/movies.service";
import Tag from "../services/movies-service/tag";
import ResponseChoices from "./responseChoices";
import {MatStepper} from "@angular/material/stepper";
import Movie from "./movie";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  tags: Tag[] = [];
  options = new Map<string, string[]>();
  panelOpenState = false;
  movies: Movie[] = [];
  spin = true;

  constructor(private moviesService: MoviesService, private changeDetectorRef: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.moviesService.getTags().subscribe(data => {
      this.tags = data;
      this.changeDetectorRef.detectChanges();
    });
  }

  onUpdateSelectedMultipleItems(tagName: string, selectedItems: string[]) {
    this.options.set(tagName, selectedItems);
  }

  onUpdateSelectedSingleItem(tagName: string, selectedItem: string) {
    this.options.set(tagName, [selectedItem]);
  }

  createOptionsObject() {
    const selectedOptions: ResponseChoices[] = [];

    this.options.forEach((values, key) => {
      selectedOptions.push({
        key, values
      });
    });

    return selectedOptions;
  }

  onGetRecommendationButtonClick() {
    this.spin = true;
    this.movies = [];

    this.moviesService.getRecommendations(this.createOptionsObject()).subscribe(data => {
      this.movies = data;
      this.spin = false;
    });
  }

  goBack(stepper: MatStepper) {
    stepper.previous();
  }

  goForward(stepper: MatStepper) {
    stepper.next();
  }

  isDisabled(tag: Tag) {
    if (tag.optional) {
      return false;
    }

    return !this.options.has(tag.name) || this.options.get(tag.name)?.length == 0;
  }

  filterValues(values: string[], filter: string) {
    if (!filter) {
      return values;
    }

    return values.filter(value => value.toLowerCase().includes(filter.toLowerCase()));
  }
}
