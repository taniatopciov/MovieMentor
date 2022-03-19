import {Component} from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

  genreListOptions: string[] = ['Comedy', 'Drama', 'Action', 'Romance', 'Animation'];
  directorListOptions: string[] = ['Christopher Nolan', 'Steven Spielberg', 'Quentin Tarantino', 'Martin Scorsese', 'David Fincher'];
  castListOptions: string[] = ['Adam Sandler', 'Angelina Jolie', 'Alicia Vikander', 'Kit Harrington', 'Zendaya'];
  durationListOptions: string[] = ['short (< 90 min)', 'medium (90 min - 120 min)', 'long ( > 120 min)'];
  yearListOptions: string[] = ['2022', '2020-2021', 'Last 5 years', 'Last 10 Years', '2000s'];
  awardsListOptions: string[] = ['Best Picture', 'Best Director', 'Best Actor', 'Best Actress', 'Best Screenplay'];
  countryOfOriginListOptions: string[] = ['America', 'Romania', 'Germany', 'France', 'Spain'];

  onUpdateSelectedGenres(selectedItems: string[]) {
    console.log(selectedItems);
  }

  onUpdateSelectedDirectors(selectedItems: string[]) {
    console.log(selectedItems);
  }

  onUpdateSelectedCast(selectedItems: string[]) {
    console.log(selectedItems);
  }

  onUpdateSelectedDuration(selectedItem: string) {
    console.log(selectedItem);
  }

  onUpdateSelectedYear(selectedItem: string) {
    console.log(selectedItem);
  }

  onUpdateSelectedAwards(selectedItems: string[]) {
    console.log(selectedItems);
  }

  onUpdateSelectedCountries(selectedItems: string[]) {
    console.log(selectedItems);
  }
}
