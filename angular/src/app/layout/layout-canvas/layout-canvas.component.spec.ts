import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayoutCanvasComponent } from './layout-canvas.component';

describe('LayoutCanvasComponent', () => {
  let component: LayoutCanvasComponent;
  let fixture: ComponentFixture<LayoutCanvasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LayoutCanvasComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LayoutCanvasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
