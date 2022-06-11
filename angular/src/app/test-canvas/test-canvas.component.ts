import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { LayoutCanvasComponent } from '../layout/layout-canvas/layout-canvas.component';

@Component({
  selector: 'app-test-canvas',
  templateUrl: './test-canvas.component.html',
  styleUrls: ['./test-canvas.component.css']
})
export class TestCanvasComponent implements OnInit {

  @ViewChild("layoutCanvas") layoutCanvasRef: LayoutCanvasComponent | undefined;

  constructor(private http: HttpClient) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.layoutCanvasRef!.loadFromUrl("/assets/testLayout.json");
  }
}
