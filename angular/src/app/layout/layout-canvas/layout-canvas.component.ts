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

  layoutRender: RenderLayoutService | undefined;

  layoutData: LayoutData | undefined;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.layoutRender = new RenderLayoutService(document.getElementById('overlayCanvas')!);
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
      this.layoutRender?.render(true);
    }
  }

  drawScreen()
  {
    this.layoutRender?.render(this.isFullyLoaded());
  }

  loadFromUrl(layoutUrl: string, parametersUrl: string)
  {
    this.http.get<any>(layoutUrl).subscribe(data => {
      // Load data
      var loadLayoutService : LoadLayoutService = new LoadLayoutService();
      this.layoutData = loadLayoutService.fromJsonContainer(data);
      this.layoutRender?.setLayout(this.layoutData);

      // Run http fetches in parallel
      for (const imageRes of this.layoutData.imageResources.values()) {
        imageRes.imageElement = new Image();
        imageRes.imageElement.addEventListener('load', () => {
          this.drawScreenIfFullyLoaded();
        });
        imageRes.imageElement.src = imageRes.url;
      }

      // Run http fetches in parallel
      for (const fontIndex in this.layoutData.fontResources) {
        var fontRes: LayoutFontResource = this.layoutData.fontResources[fontIndex];
        var fontFace = new FontFace(fontRes.fontName, 'url(' + fontRes.url + ')');
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
