import type { } from "css-font-loading-module";

import { OverlayApiDto } from '../model/layout-api-dto';
import { LayoutData, LayoutProperty, LayoutImageResource, LayoutFontResource, LayoutAudioResource, LayoutVideoResource, LayoutFontConfig, LayoutAudioConfig } from '../layout-data';

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
    if (layoutData.audioResources !== undefined && layoutData.audioResources !== null)
    {
      resourcesToLoad += layoutData.audioResources.length;
    }
    if (layoutData.videoResources !== undefined && layoutData.videoResources !== null)
    {
      resourcesToLoad += layoutData.videoResources.length;
    }

    // We go straight away if there are no resources
    if (resourcesToLoad === 0)
    {
      resourcesToLoad = 1;
      successCallback(layoutData);
      return;
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
    if (layoutData.audioResources !== undefined && layoutData.audioResources !== null)
    {
      for (const audioIndex in layoutData.audioResources) {
        var audioRes: LayoutAudioResource = layoutData.audioResources[audioIndex];
        audioRes.audioElement = new Audio(audioRes.url);
        --resourcesToLoad;
        if (resourcesToLoad === 0 && isErrored === false)
        {
          successCallback(layoutData);
        }
      }
    }
    if (layoutData.videoResources !== undefined && layoutData.videoResources !== null)
    {
      for (const videoIndex in layoutData.videoResources) {
        var videoRes: LayoutVideoResource = layoutData.videoResources[videoIndex];
        videoRes.videoElement = document.createElement('video');
        videoRes.videoElement.className = "overlayVideo";
        videoRes.videoElement.src = videoRes.url;
        --resourcesToLoad;
        if (resourcesToLoad === 0 && isErrored === false)
        {
          successCallback(layoutData);
        }
      }
    }
  }

  fromJson(apiDto: OverlayApiDto) : LayoutData
  {
    var layoutData : LayoutData = new LayoutData();
    layoutData.canvasSize = apiDto.canvasSize;
    if (apiDto.redrawMillis !== null)
      layoutData.redrawMillis = apiDto.redrawMillis;
    if (apiDto.apiFetchMillis !== null)
      layoutData.apiFetchMillis = apiDto.apiFetchMillis;
    if (apiDto.properties !== undefined)
    {
      for (var propertyIndex in apiDto.properties) {
        var newProp = new LayoutProperty();
        newProp.propDef = apiDto.properties[propertyIndex];
        newProp.type = newProp.propDef.type;
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
    if (apiDto.audioResources !== undefined)
    {
      for (var audioResIndex in apiDto.audioResources) {
        var audioUrl = apiDto.audioResources[audioResIndex];
        layoutData.audioResources.push(new LayoutAudioResource(audioUrl));
      }
    }
    if (apiDto.videoResources !== undefined)
    {
      for (var videoResIndex in apiDto.videoResources) {
        var videoUrl = apiDto.videoResources[videoResIndex];
        layoutData.videoResources.push(new LayoutVideoResource(videoUrl));
      }
    }
    if (apiDto.fonts !== undefined)
    {
      for (var fontJsonIndex in apiDto.fonts) {
        var fontJson: any = apiDto.fonts[fontJsonIndex];
        layoutData.fontConfigs.push(new LayoutFontConfig(layoutData.fontResources[fontJson.res], fontJson));
      }
    }
    if (apiDto.audios !== undefined)
    {
      for (var audioJsonIndex in apiDto.audios) {
        var audioJson: any = apiDto.audios[audioJsonIndex];
        layoutData.audioConfigs.push(new LayoutAudioConfig(layoutData.audioResources[audioJson.res], audioJson.prop));
      }
    }
    layoutData.rootElement = apiDto.rootElement;
    return layoutData;
  }
}
