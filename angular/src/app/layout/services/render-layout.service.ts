import { LayoutData, LayoutFontConfig } from '../layout-data';

enum RenderLayoutSizeType { Pixel, Min, Max }

class RenderLayoutSize {
  widthType: RenderLayoutSizeType = RenderLayoutSizeType.Pixel;
  width: number = 0;
  heightType: RenderLayoutSizeType = RenderLayoutSizeType.Pixel;
  height: number = 0;
}

class LayoutElementWithBounds {
  element: any | undefined;
  left: number = 0;
  right: number = 0;
  top: number = 0;
  bottom: number = 0;
  children: LayoutElementWithBounds[] = [];
}

export class RenderLayoutService {
  context: CanvasRenderingContext2D | undefined;
  layoutData: LayoutData | undefined;
  rootElementWithBounds: LayoutElementWithBounds | undefined;

  constructor(context: CanvasRenderingContext2D) {
    this.context = context;
  }

  setLayout(layoutData: LayoutData)
  {
    this.layoutData = layoutData;
    this.rootElementWithBounds = this.createElementWithBounds(layoutData, layoutData.rootElement);
  }

  createElementWithBounds(layoutData: LayoutData, element: any): LayoutElementWithBounds
  {
    var elementWithBounds: LayoutElementWithBounds = new LayoutElementWithBounds();
    elementWithBounds.element = element;
    element.children?.forEach((item: any) => {
      elementWithBounds.children.push(this.createElementWithBounds(layoutData, item));
    });
    return elementWithBounds;
  }

  getString(layoutData: LayoutData, object: any) {
    if (typeof object === "string")
    {
      return object;
    }
    if (object.op === "para")
    {
      if (layoutData.parameters.has(object.name))
        return layoutData.parameters.get(object.name)!.value;
      return "<" + object.name + ">";
    }
    if (object.op === "cat")
    {
      var result = "";
      for (var valueIndex in object.values) {
        result += this.getString(layoutData, object.values[valueIndex]);
      }
      return result;
    }
    return object;
  }

  measureRootElementWithBounds(width: number, height: number)
  {
    this.rootElementWithBounds!.right = width;
    this.rootElementWithBounds!.bottom = height;
    this.rootElementWithBounds!.children.forEach((item: LayoutElementWithBounds) => {
      this.measureElementWithBounds(this.rootElementWithBounds!, item);
    });
  }

  measureElementWithBounds(parentElement: LayoutElementWithBounds, childElement: LayoutElementWithBounds)
  {
    // Copy from parent
    childElement.left = 0;
    childElement.top = 0;
    var parentWidth = parentElement.right - parentElement.left;
    var parentHeight = parentElement.bottom - parentElement.top;
    childElement.right = parentWidth;
    childElement.bottom = parentHeight;
    // Apply pixel margins and constant widths
    if (typeof childElement.element.marginLeft == "number")
    {
      childElement.left += childElement.element.marginLeft;
    }
    if (typeof childElement.element.marginRight == "number")
    {
      childElement.right -= childElement.element.marginRight;
    }
    if (typeof childElement.element.marginTop == "number")
    {
      childElement.top += childElement.element.marginTop;
    }
    if (typeof childElement.element.marginBottom == "number")
    {
      childElement.bottom -= childElement.element.marginBottom;
    }
    if (typeof childElement.element.width == "number")
    {
      var newRight : number = childElement.left + childElement.element.width;
      childElement.right = (childElement.right < newRight) ? childElement.right : newRight;
    }
    if (typeof childElement.element.height == "number")
    {
      var newTop : number = childElement.top + childElement.element.height;
      childElement.bottom = (childElement.bottom < newTop) ? childElement.bottom : newTop;
    }
    // Do children
    childElement.children.forEach((item: LayoutElementWithBounds) => {
      this.measureElementWithBounds(childElement, item);
    });
    // Apply min and max heights
    if (childElement.element.width === "Min")
    {
      // TODO
    }
    if (childElement.element.height === "Min")
    {
      // TODO
    }
    // Apply alignment margins
    if (childElement.element.marginLeft === "Max")
    {
      var offset : number = parentWidth - childElement.right;
      childElement.left += offset;
      childElement.right += offset;
    }
    if (childElement.element.marginTop === "Max")
    {
      var offset : number = parentHeight - childElement.bottom;
      childElement.top += offset;
      childElement.bottom += offset;
    }
  }

  render(isLoaded: boolean) : void
  {
    var canvasWidth = 1920;
    var canvasHeight = 1080;
    if (isLoaded === false|| this.rootElementWithBounds === undefined)
    {
      this.renderLoading(canvasWidth, canvasHeight);
    }
    else
    {
      this.measureRootElementWithBounds(canvasWidth, canvasHeight);
      console.log("Bounds: " + JSON.stringify(this.rootElementWithBounds!));
      this.context!.clearRect(0, 0, canvasWidth, canvasHeight);
      this.renderElement(this.rootElementWithBounds!);
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

  renderElement(elementWithBounds: LayoutElementWithBounds)
  {
    switch (elementWithBounds.element.elTyp)
    {
      case "Stack":
        this.renderElementStack(elementWithBounds);
        break;
      case "Image":
        this.renderElementImage(elementWithBounds);
        break;
      case "Text":
        this.renderElementText(elementWithBounds);
        break;
      case "Colour":
        this.renderElementColour(elementWithBounds);
        break;
    }
  }

  renderElementStack(elementWithBounds: LayoutElementWithBounds)
  {
    elementWithBounds.children.forEach((item: any) => {
      this.renderElement(item);
    });
  }

  renderElementImage(elementWithBounds: LayoutElementWithBounds)
  {
    var image: HTMLImageElement = this.layoutData!.imageResources.get(elementWithBounds.element.res)!.imageElement!;
    if (elementWithBounds.element.fit === "Tiled") {
      this.renderElementImageTiled(image, elementWithBounds);
    }
    else {
      this.renderElementImageSimple(image, elementWithBounds);
    }
  }

  renderElementImageSimple(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds)
  {
    var width : number = elementWithBounds.right - elementWithBounds.left;
    var height : number = elementWithBounds.bottom - elementWithBounds.top;
    this.context!.drawImage(image, 0, 0, width, height, elementWithBounds.left, elementWithBounds.top, width, height);
  }

  renderElementImageTiled(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds)
  {
    let imageWidth = image.width;
    let imageHeight = image.height;
    if (imageWidth <= 0 || imageHeight <= 0)
    {
      return;
    }
    let curTop = 0;
    var left : number = elementWithBounds.left;
    var top : number = elementWithBounds.top;
    var width : number = elementWithBounds.right - elementWithBounds.left;
    var height : number = elementWithBounds.bottom - elementWithBounds.top;
    while (curTop < height)
    {
      let drawHeight = (height < curTop + imageHeight) ? (height - curTop) : imageHeight;
      let curLeft = 0;
      while (curLeft < width)
      {
        let drawWidth = (width < curLeft + imageWidth) ? (width - curLeft) : imageWidth;
        this.context!.drawImage(image, 0, 0, drawWidth, drawHeight, left + curLeft, top + curTop, drawWidth, drawHeight);
        curLeft += imageWidth;
      }
      curTop += imageHeight;
    }
  }

  renderElementText(elementWithBounds: LayoutElementWithBounds)
  {
    var fontConfig: LayoutFontConfig = this.layoutData!.fontConfigs.get(elementWithBounds.element.font)!;
    this.context!.fillStyle = fontConfig.fontJson.colour;
    this.context!.font = fontConfig.fontJson.size + ' ' + fontConfig.fontJson.fontRes;
    switch (elementWithBounds.element.textAlign)
    {
      case "Center":
        this.context!.textAlign = 'center';
        this.context!.fillText(this.getString(this.layoutData!, elementWithBounds.element.text), (elementWithBounds.left + elementWithBounds.right) / 2, elementWithBounds.top);
        break;
      case "Right":
        this.context!.textAlign = 'right';
        this.context!.fillText(this.getString(this.layoutData!, elementWithBounds.element.text), elementWithBounds.right, elementWithBounds.top);
        break;
      default:
        this.context!.textAlign = 'left';
        this.context!.fillText(this.getString(this.layoutData!, elementWithBounds.element.text), elementWithBounds.left, elementWithBounds.top);
        break;
    }
  }

  renderElementColour(elementWithBounds: LayoutElementWithBounds)
  {
    var left : number = elementWithBounds.left;
    var top : number = elementWithBounds.top;
    var width : number = elementWithBounds.right - elementWithBounds.left;
    var height : number = elementWithBounds.bottom - elementWithBounds.top;
    this.context!.fillStyle = elementWithBounds.element.colour;
    this.context!.fillRect(left, top, width, height);
  }
}
