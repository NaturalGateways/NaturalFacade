import { LayoutData } from '../layout-data';

export class RenderLayoutService {
  context: CanvasRenderingContext2D | undefined;

  constructor(context: CanvasRenderingContext2D) {
    this.context = context;
  }

  render(layoutData: LayoutData | undefined) : void
  {
    var canvasWidth = 1920;
    var canvasHeight = 1080;
    if (layoutData === undefined)
    {
      this.renderLoading(canvasWidth, canvasHeight);
    }
    else
    {
      this.context!.clearRect(0, 0, canvasWidth, canvasHeight);
      this.renderElement(layoutData.rootElement, 0, 0, canvasWidth, canvasHeight);
    }
  }

  renderLoading(canvasWidth: number, canvasHeight: number)
  {
    this.context!.fillStyle = '#003366';
    this.context!.fillRect(0, 0, canvasWidth, canvasHeight);
    this.context!.fillStyle = '#FFFFFF';
    this.context!.font = '28px Helvetica';
    this.context!.fillText('Loading...', 20, 50);
  }

  renderElement(element: any, left: number, top: number, width: number, height: number)
  {
    switch (element.elTyp)
    {
      case "stack":
        this.renderElementStack(element, left, top, width, height);
        break;
      case "colour":
        this.renderElementColour(element, left, top, width, height);
        break;
    }
  }

  renderElementStack(element: any, left: number, top: number, width: number, height: number)
  {
    element.children.forEach((item: any) => {
      this.renderElementColour(item, left, top, width, height);
  });
  }

  renderElementColour(element: any, left: number, top: number, width: number, height: number)
  {
    this.context!.fillStyle = element.colour;
    this.context!.fillRect(left, top, width, height);
  }
}
