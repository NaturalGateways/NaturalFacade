import { Component, OnInit } from '@angular/core';

import { LoadLayoutService } from '../services/load-layout.service';
import { RenderLayoutService } from '../services/render-layout.service';
import { LayoutData } from '../layout-data';

@Component({
  selector: 'layout-canvas',
  templateUrl: './layout-canvas.component.html',
  styleUrls: ['./layout-canvas.component.css']
})
export class LayoutCanvasComponent implements OnInit {

  layourRender: RenderLayoutService | undefined;

  layoutData: LayoutData | undefined;

  constructor() { }

  ngOnInit(): void {
    var canvas: HTMLCanvasElement = document.getElementById('overlayCanvas') as HTMLCanvasElement;
    var context: CanvasRenderingContext2D = canvas!.getContext("2d")!;
    this.layourRender = new RenderLayoutService(context);

    this.drawScreen();
  }

  drawScreen()
  {
    this.layourRender?.render(this.layoutData);
  }

  loadFromString(jsonString: string)
  {
    var loadLayoutService : LoadLayoutService = new LoadLayoutService();
    this.layoutData = loadLayoutService.fromString(jsonString);
    this.drawScreen();
  }
}
