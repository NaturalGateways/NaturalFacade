import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';

import { LayoutCanvasComponent } from '../layout/layout-canvas/layout-canvas.component';

@Component({
  selector: 'app-test-canvas',
  templateUrl: './test-canvas.component.html',
  styleUrls: ['./test-canvas.component.css']
})
export class TestCanvasComponent implements OnInit {

  @ViewChild("layoutCanvas") layoutCanvasRef: LayoutCanvasComponent | undefined;

  constructor() { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.layoutCanvasRef!.loadFromString("{\"resources\":[],\"fonts\":[],\"rootElement\":{\"elTyp\":\"stack\",\"children\":[{\"elTyp\":\"colour\",\"colour\":\"#FF00FF\"},{\"elTyp\":\"colour\",\"colour\":\"#00FF00\"}]}}");
  }
}
