export class LayoutApiDtoContainer {
  Payload: OverlayApiDto = new OverlayApiDto();
}

export class ConvertedOverlayApiDto {
  overlay: OverlayApiDto | undefined;

  propValues: any;
}

export class OverlayApiDto {
  canvasSize: any = [];

  redrawMillis: number | undefined;

  apiFetchMillis: number | undefined;

  properties: any = [];

  imageResources: any = [];

  fontResources: any = [];

  fonts: any = [];

  rootElement: any;
}
