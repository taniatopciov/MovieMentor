import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SingleSelectionListComponent } from './single-selection-list.component';

describe('SingleSelectionListComponent', () => {
  let component: SingleSelectionListComponent;
  let fixture: ComponentFixture<SingleSelectionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SingleSelectionListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SingleSelectionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
