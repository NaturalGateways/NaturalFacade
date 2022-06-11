import { HttpClient } from '@angular/common/http';

import { LayoutApiDto } from '../model/layout-api-dto';
import { LayoutData, LayoutImageResource } from '../layout-data';

export class LoadLayoutService {

  constructor() { }

  fromJson(apiDto: LayoutApiDto) : LayoutData
  {
    var layoutData : LayoutData = new LayoutData();
    if (apiDto.imageResources !== undefined)
    {
      for (var key in apiDto.imageResources) {
        layoutData.imageResources.set(key, new LayoutImageResource(apiDto.imageResources[key]));
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
}
