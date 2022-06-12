export class LayoutData {
  parameters: Map<string, LayoutParameter> = new Map<string, LayoutParameter>();

  parametersLoaded: boolean = false;

  imageResources: Map<string, LayoutImageResource> = new Map<string, LayoutImageResource>();

  fontResources: Map<string, LayoutFontResource> = new Map<string, LayoutFontResource>();

  fontConfigs: Map<string, LayoutFontConfig> = new Map<string, LayoutFontConfig>();

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

  constructor(public url: string) { }
}

export class LayoutFontConfig {
  isCustom: boolean = false;

  constructor(public fontJson: any) { }
}
