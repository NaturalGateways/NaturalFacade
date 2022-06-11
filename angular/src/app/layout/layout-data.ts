export class LayoutData {
  imageResources: Map<string, LayoutImageResource> = new Map<string, LayoutImageResource>();

  rootElement: any;
}

export class LayoutImageResource {
  imageElement: HTMLImageElement | undefined;

  constructor(public url: string) { }
}
