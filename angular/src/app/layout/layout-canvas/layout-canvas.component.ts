import type { } from "css-font-loading-module";

import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { LoadLayoutService } from '../services/load-layout.service';
import { RenderLayoutService } from '../services/render-layout.service';
import { LayoutData, LayoutParameter, LayoutFontResource } from '../layout-data';

@Component({
  selector: 'layout-canvas',
  templateUrl: './layout-canvas.component.html',
  styleUrls: ['./layout-canvas.component.css']
})
export class LayoutCanvasComponent implements OnInit {

  layourRender: RenderLayoutService | undefined;

  layoutData: LayoutData | undefined;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    var canvas: HTMLCanvasElement = document.getElementById('overlayCanvas') as HTMLCanvasElement;
    var context: CanvasRenderingContext2D = canvas!.getContext("2d")!;
    this.layourRender = new RenderLayoutService(context);

    this.drawScreen();
  }

  isFullyLoaded(): boolean
  {
    if (this.layoutData!.parametersLoaded === false) {
      return false;
    }
    for (const imageRes of this.layoutData!.imageResources.values()) {
      if (imageRes.imageElement === undefined || imageRes.imageElement.complete === false) {
        return false;
      }
    }
    for (const fontRes of this.layoutData!.fontResources.values()) {
      if (fontRes.loaded === false) {
        return false;
      }
    }
    return true;
  }

  drawScreenIfFullyLoaded()
  {
    if (this.isFullyLoaded()) {
      this.drawScreen();
    }
  }

  drawScreen()
  {
    this.layourRender?.render(this.layoutData);
  }

  loadFromUrl(layoutUrl: string, parametersUrl: string)
  {
    this.http.get<any>(layoutUrl).subscribe(data => {
      // Load data
      var loadLayoutService : LoadLayoutService = new LoadLayoutService();
      this.layoutData = loadLayoutService.fromJson(data);

      // Run http fetches in parallel
      for (const imageRes of this.layoutData.imageResources.values()) {
        imageRes.imageElement = new Image();
        imageRes.imageElement.addEventListener('load', () => {
          this.drawScreenIfFullyLoaded();
        });
        imageRes.imageElement.src = imageRes.url;
      }

      // Run http fetches in parallel
      for (const fontResKey of this.layoutData.fontResources.keys()) {
        var fontRes: LayoutFontResource = this.layoutData.fontResources.get(fontResKey)!;
        var fontFace = new FontFace(fontResKey, 'url(' + fontRes.url + ')');
        fontFace.load().then((font) => {
          document.fonts.add(font);
          fontRes.loaded = true;
          this.drawScreenIfFullyLoaded();
        });
      }

      // Load parameters
      this.http.get<{[key: string]: any;}>(parametersUrl).subscribe(data => {
        // Set parameters
        loadLayoutService.loadParametersFromData(this.layoutData, data);

        // Redraw
        this.drawScreenIfFullyLoaded();
      });
    });
  }

  loadParametersFromUrl(url: string) {
    this.http.get<{[key: string]: any;}>(url).subscribe(data => {
      // Set parameters
      var parameters: Map<string, LayoutParameter> | undefined = this.layoutData?.parameters;
      if (parameters === undefined)
      {
        return;
      }

      // Set parameters
      for (var key in data) {
        if (parameters!.has(key)) {
          parameters.get(key)!.value = data[key]!;
        }
      }

      // Redraw
      this.drawScreenIfFullyLoaded();
    });
  }
}
