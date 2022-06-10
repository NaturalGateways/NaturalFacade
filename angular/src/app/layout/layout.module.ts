import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutCanvasComponent } from './layout-canvas/layout-canvas.component';

export { LayoutCanvasComponent } from './layout-canvas/layout-canvas.component';

@NgModule({
  declarations: [
    LayoutCanvasComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    LayoutCanvasComponent
  ]
})
export class LayoutModule { }
