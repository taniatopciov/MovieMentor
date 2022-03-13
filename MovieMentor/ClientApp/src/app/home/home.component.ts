import {Component} from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

  yearListOptions: string[] = ['2022', '2020-2021', 'Last 5 years', 'Last 10 Years', '2000s'];

  onUpdateSelectedYear(selectedItem: string) {
    console.log(selectedItem);
  }
}
