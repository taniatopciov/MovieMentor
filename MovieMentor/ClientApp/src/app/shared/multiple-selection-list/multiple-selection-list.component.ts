import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'multiple-selection-list',
  templateUrl: './multiple-selection-list.component.html',
  styleUrls: ['./multiple-selection-list.component.css']
})
export class MultipleSelectionListComponent {

  @Input() listData: string[];

  selectedOptions: string[];

  @Output() updateSelectedListItem = new EventEmitter<string[]>();

  constructor() {
    this.listData = [];
    this.selectedOptions = [];
  }

  selectItem(currentSelection: string) {
    let found = false;

    this.selectedOptions.forEach((option, index) => {
      if (option === currentSelection) {
        found = true;
        this.selectedOptions.splice(index, 1);
      }
    });

    if (!found) {
      this.selectedOptions.push(currentSelection);
    }

    this.updateSelectedListItem.emit(this.selectedOptions);
  }
}
