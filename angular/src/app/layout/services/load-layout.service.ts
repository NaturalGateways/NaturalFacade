import { LayoutData } from '../layout-data';

export class LoadLayoutService {

  constructor() { }

  fromString(jsonString: string) : LayoutData
  {
    var jsonObj = JSON.parse(jsonString);
    var layoutData : LayoutData = new LayoutData();
    layoutData.rootElement = jsonObj.rootElement;
    return layoutData;
  }
}
