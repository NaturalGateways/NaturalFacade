export class LayoutApiDtoContainer {
  Payload: OverlayApiDto = new OverlayApiDto();
}

export class ConvertedOverlayApiDto {
  overlay: OverlayApiDto | undefined;

  propValues: any;
}

export class OverlayApiDto {
  canvasSize: any = [];

  properties: any = [];

  imageResources: any = [];

  fontResources: any = [];

  fonts: any = [];

  rootElement: any;
}
