export class LayoutApiDtoContainer {
  Payload: LayoutApiDto = new LayoutApiDto();
}

export class LayoutApiDto {
  parameters: { [key: string]: any } = {};

  imageResources: { [key: string]: string } = {};

  fontResources: { [key: string]: string } = {};

  fonts: { [key: string]: any } = {};

  rootElement: any;
}
