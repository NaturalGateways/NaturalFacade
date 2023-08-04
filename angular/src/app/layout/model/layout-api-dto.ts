export class LayoutApiDtoContainer {
  Payload: OverlayApiDto = new OverlayApiDto();
}

export class ConvertedOverlayApiDto {
  overlay: OverlayApiDto | undefined;

  propValues: any;
}

export class OverlayApiDto {
  canvasSize: any = [];

  redrawMillis: number | null = null;

  apiFetchMillis: number | null = null;

  properties: any = [];

  imageResources: any = [];

  fontResources: any = [];

  audioResources: any = [];

  fonts: any = [];

  audios: any = [];

  rootElement: any;
}
