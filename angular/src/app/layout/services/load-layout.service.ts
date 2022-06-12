import { HttpClient } from '@angular/common/http';

import { LayoutApiDto } from '../model/layout-api-dto';
import { LayoutData, LayoutParameter, LayoutImageResource, LayoutFontResource, LayoutFontConfig } from '../layout-data';

export class LoadLayoutService {

  constructor() { }

  fromJson(apiDto: LayoutApiDto) : LayoutData
  {
    var layoutData : LayoutData = new LayoutData();
    if (apiDto.parameters !== undefined)
    {
      for (var key in apiDto.parameters) {
        layoutData.parameters.set(key, new LayoutParameter());
      }
    }
    if (apiDto.imageResources !== undefined)
    {
      for (var key in apiDto.imageResources) {
        layoutData.imageResources.set(key, new LayoutImageResource(apiDto.imageResources[key]));
      }
    }
    if (apiDto.fontResources !== undefined)
    {
      for (var key in apiDto.fontResources) {
        layoutData.fontResources.set(key, new LayoutFontResource(apiDto.fontResources[key]));
      }
    }
    if (apiDto.fonts !== undefined)
    {
      for (var key in apiDto.fonts) {
        layoutData.fontConfigs.set(key, new LayoutFontConfig(apiDto.fonts[key]));
      }
    }
    layoutData.rootElement = apiDto.rootElement;
    return layoutData;
  }

  fromString(jsonString: string) : LayoutData
  {
    var jsonObj: LayoutApiDto = JSON.parse(jsonString);
    return this.fromJson(jsonObj);
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
