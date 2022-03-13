import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'single-selection-list',
  templateUrl: './single-selection-list.component.html',
  styleUrls: ['./single-selection-list.component.css']
})
export class SingleSelectionListComponent {

  @Input() listData: string[];

  @Output() updateSelectedListItem = new EventEmitter<string>();

  selectedItem: string = "";

  constructor() {
    this.listData = [];
  }

  selectItem(currentSelection: string) {
    this.updateSelectedListItem.emit(currentSelection);
  }
}
