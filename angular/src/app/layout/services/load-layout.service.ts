import type { } from "css-font-loading-module";

import { OverlayApiDto } from '../model/layout-api-dto';
import { LayoutData, LayoutProperty, LayoutImageResource, LayoutFontResource, LayoutFontConfig } from '../layout-data';

export class LoadLayoutService {

  constructor() { }

  loadAllFromJson(apiDto: OverlayApiDto, successCallback: (layoutData: LayoutData) => void, errorCallback: () => void)
  {
    // Load data
    var layoutData: LayoutData = this.fromJson(apiDto);

    // Count resources to load
    var resourcesToLoad: number = 0;
    var isErrored: boolean = false;
    if (layoutData.imageResources !== undefined && layoutData.imageResources !== null)
    {
      resourcesToLoad += layoutData.imageResources.length;
    }
    if (layoutData.fontResources !== undefined && layoutData.fontResources !== null)
    {
      resourcesToLoad += layoutData.fontResources.length;
    }

    // Load images in parallel
    if (layoutData.imageResources !== undefined && layoutData.imageResources !== null)
    {
      for (const imageRes of layoutData.imageResources.values()) {
        imageRes.imageElement = new Image();
        imageRes.imageElement.addEventListener('load', () => {
          --resourcesToLoad;
          if (resourcesToLoad === 0 && isErrored === false)
          {
            successCallback(layoutData);
          }
        });
        imageRes.imageElement.src = imageRes.url;
      }
    }
    if (layoutData.fontResources !== undefined && layoutData.fontResources !== null)
    {
      for (const fontIndex in layoutData.fontResources) {
        var fontRes: LayoutFontResource = layoutData.fontResources[fontIndex];
        var fontFace = new FontFace(fontRes.fontName, 'url(' + fontRes.url + ')');
        fontFace.load().then((font) => {
          document.fonts.add(font);
          fontRes.loaded = true;
          --resourcesToLoad;
          if (resourcesToLoad === 0 && isErrored === false)
          {
            successCallback(layoutData);
          }
        });
      }
    }
  }

  fromJson(apiDto: OverlayApiDto) : LayoutData
  {
    var layoutData : LayoutData = new LayoutData();
    layoutData.canvasSize = apiDto.canvasSize;
    if (apiDto.properties !== undefined)
    {
      for (var propertyIndex in apiDto.properties) {
        var newProp = new LayoutProperty();
        newProp.type = apiDto.properties[propertyIndex].type;
        layoutData.properties.push(newProp);
      }
    }
    if (apiDto.imageResources !== undefined)
    {
      for (var imageIndex in apiDto.imageResources) {
        layoutData.imageResources.push(new LayoutImageResource(apiDto.imageResources[imageIndex]));
      }
    }
    if (apiDto.fontResources !== undefined)
    {
      for (var fontResIndex in apiDto.fontResources) {
        var fontName: string = 'overFont' + fontResIndex;
        var fontUrl = apiDto.fontResources[fontResIndex];
        layoutData.fontResources.push(new LayoutFontResource(fontName, fontUrl));
      }
    }
    if (apiDto.fonts !== undefined)
    {
      for (var fontJsonIndex in apiDto.fonts) {
        var fontJson: any = apiDto.fonts[fontJsonIndex];
        layoutData.fontConfigs.push(new LayoutFontConfig(layoutData.fontResources[fontJson.res], fontJson));
      }
    }
    layoutData.rootElement = apiDto.rootElement;
    return layoutData;
  }
}
