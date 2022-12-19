import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditControlsFieldComponent } from './edit-controls-field.component';

describe('EditControlsFieldComponent', () => {
  let component: EditControlsFieldComponent;
  let fixture: ComponentFixture<EditControlsFieldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditControlsFieldComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditControlsFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
