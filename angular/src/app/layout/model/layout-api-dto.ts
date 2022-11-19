export class LayoutApiDtoContainer {
  Payload: LayoutApiDto = new LayoutApiDto();
}

export class LayoutApiDto {
  parameters: { [key: string]: any } = {};

  imageResources: any = [];

  fontResources: any = [];

  fonts: any = [];

  rootElement: any;
}
