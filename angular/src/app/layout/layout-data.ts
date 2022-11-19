export class LayoutData {
  parameters: Map<string, LayoutParameter> = new Map<string, LayoutParameter>();

  parametersLoaded: boolean = false;

  imageResources: Array<LayoutImageResource> = new Array<LayoutImageResource>();

  fontResources: Array<LayoutFontResource> = new Array<LayoutFontResource>();

  fontConfigs: Array<LayoutFontConfig> = new Array<LayoutFontConfig>();

  rootElement: any;
}

export class LayoutParameter {
  value: any | undefined;

  constructor() { }
}

export class LayoutImageResource {
  imageElement: HTMLImageElement | undefined;

  constructor(public url: string) { }
}

export class LayoutFontResource {
  loaded: boolean = false;

  constructor(public fontName: string, public url: string) { }
}

export class LayoutFontConfig {
  isCustom: boolean = false;

  fontName: string = '';

  constructor(public fontRes: LayoutFontResource, public fontJson: any)
  {
    this.fontName = this.fontJson.size + ' ' + this.fontRes.fontName;
  }
}
