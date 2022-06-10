import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-canvas',
  templateUrl: './test-canvas.component.html',
  styleUrls: ['./test-canvas.component.css']
})
export class TestCanvasComponent implements OnInit {

  canvas: HTMLCanvasElement | undefined;
  context: CanvasRenderingContext2D | undefined;

  constructor() { }

  ngOnInit(): void {
  }
}
