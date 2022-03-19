import {Component, OnInit} from '@angular/core';
import {MoviesService} from "../services/movies-service/movies.service";
import Tag from "../services/movies-service/tag";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  tags: Tag[] = [];
  options = new Map<string, string[]>();

  constructor(private moviesService: MoviesService) {
  }

  ngOnInit() {
    this.moviesService.getTags().subscribe(data => {
      this.tags = data;
      console.log(this.tags.length);
    })
  }

  onUpdateSelectedMultipleItems(tagName: string, selectedItems: string[]) {
    // console.log(tagName + selectedItems);
    this.options.set(tagName, selectedItems);
  }

  onUpdateSelectedSingleItem(tagName: string, selectedItem: string) {
    // console.log(tagName + selectedItem);
    this.options.set(tagName, [selectedItem]);
  }
}
