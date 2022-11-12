import { HttpClient } from '@angular/common/http';

import { LayoutApiDto, LayoutApiDtoContainer } from '../model/layout-api-dto';
import { LayoutData, LayoutParameter, LayoutImageResource, LayoutFontResource, LayoutFontConfig } from '../layout-data';

export class LoadLayoutService {

  constructor() { }

  fromJson(apiDto: LayoutApiDtoContainer) : LayoutData
  {
    var layoutData : LayoutData = new LayoutData();
    if (apiDto.Payload.parameters !== undefined)
    {
      for (var key in apiDto.Payload.parameters) {
        layoutData.parameters.set(key, new LayoutParameter());
      }
    }
    if (apiDto.Payload.imageResources !== undefined)
    {
      for (var key in apiDto.Payload.imageResources) {
        layoutData.imageResources.set(key, new LayoutImageResource(apiDto.Payload.imageResources[key]));
      }
    }
    if (apiDto.Payload.fontResources !== undefined)
    {
      for (var key in apiDto.Payload.fontResources) {
        layoutData.fontResources.set(key, new LayoutFontResource(apiDto.Payload.fontResources[key]));
      }
    }
    if (apiDto.Payload.fonts !== undefined)
    {
      for (var key in apiDto.Payload.fonts) {
        layoutData.fontConfigs.set(key, new LayoutFontConfig(apiDto.Payload.fonts[key]));
      }
    }
    layoutData.rootElement = apiDto.Payload.rootElement;
    return layoutData;
  }

  loadParametersFromData(layoutData: LayoutData | undefined, dataJson: {[key: string]: any;}) {
    // Set parameters
    var parameters: Map<string, LayoutParameter> | undefined = layoutData?.parameters;
    if (parameters === undefined)
    {
      return;
    }

    // Set parameters
    for (var key in dataJson) {
      if (parameters!.has(key)) {
        parameters.get(key)!.value = dataJson[key]!;
      }
    }
    layoutData!.parametersLoaded = true;
  }
}
