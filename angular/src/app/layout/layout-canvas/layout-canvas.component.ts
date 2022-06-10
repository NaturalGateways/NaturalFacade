import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'layout-canvas',
  templateUrl: './layout-canvas.component.html',
  styleUrls: ['./layout-canvas.component.css']
})
export class LayoutCanvasComponent implements OnInit {

  canvas: HTMLCanvasElement | undefined;
  context: CanvasRenderingContext2D | undefined;

  constructor() { }

  ngOnInit(): void {
    this.canvas = document.getElementById('overlayCanvas') as HTMLCanvasElement;
    this.context = this.canvas!.getContext("2d")!;

    this.drawScreen();
  }

  drawScreen()
  {
    this.drawLoading();
  }

  drawLoading()
  {
    var canvasWidth = 1920;
    var canvasHeight = 1080;
    this.context!.fillStyle = '#003366';
    this.context!.fillRect(0, 0, canvasWidth, canvasHeight);
    this.context!.fillStyle = '#FFFFFF';
    this.context!.font = '28px Helvetica';
    this.context!.fillText('Loading...', 20, 50);
  }
}
