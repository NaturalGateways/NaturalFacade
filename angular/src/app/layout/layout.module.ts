import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutCanvasComponent } from './layout-canvas/layout-canvas.component';
import { ViewOverlayComponent } from './view-overlay/view-overlay.component';

export { LayoutCanvasComponent } from './layout-canvas/layout-canvas.component';

@NgModule({
  declarations: [
    LayoutCanvasComponent,
    ViewOverlayComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    LayoutCanvasComponent
  ]
})
export class LayoutModule { }
