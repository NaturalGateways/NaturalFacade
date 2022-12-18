import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayoutGridItemComponent } from './layout-grid-item.component';

describe('LayoutGridItemComponent', () => {
  let component: LayoutGridItemComponent;
  let fixture: ComponentFixture<LayoutGridItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LayoutGridItemComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LayoutGridItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
