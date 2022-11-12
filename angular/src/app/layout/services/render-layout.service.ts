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
  minWidth: number = 0;
  minHeight: number = 0;
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
    this.rootElementWithBounds!.left = 0;
    this.rootElementWithBounds!.top = 0;
    this.rootElementWithBounds!.right = width;
    this.rootElementWithBounds!.bottom = height;
    this.measureElementMinimumSize(this.rootElementWithBounds!);
    //console.log("Min: " + JSON.stringify(this.rootElementWithBounds!));
    this.measureElementBounds(this.rootElementWithBounds!);
    //console.log("Bounds: " + JSON.stringify(this.rootElementWithBounds!));
  }

  measureElementMinimumSize(element: LayoutElementWithBounds)
  {
    switch (element.element.elTyp)
    {
      case "Image":
        this.measureImageElementMinimumSize(element);
        break;
      case "Stack":
        this.measureStackElementMinimumSize(element);
        break;
    }
  }

  measureImageElementMinimumSize(elementWithBounds: LayoutElementWithBounds)
  {
    var minWidth = 0;
    var minHeight = 0;
    if (elementWithBounds.element.fit === "None")
    {
      var image: HTMLImageElement = this.layoutData!.imageResources.get(elementWithBounds.element.res)!.imageElement!;
      minWidth = image.width;
      minHeight = image.height;
    }
    elementWithBounds.minWidth = minWidth;
    elementWithBounds.minHeight = minHeight;
  }

  measureStackElementMinimumSize(element: LayoutElementWithBounds)
  {
    var parentMinWidth = 0;
    var parentMinHeight = 0;
    element.children.forEach((child: LayoutElementWithBounds) => {
      this.measureElementMinimumSize(child);
      // Apply fixed widths and heights
      if (typeof child.element.width == "number")
      {
        child.minWidth = child.element.width;
      }
      if (typeof child.element.height == "number")
      {
        child.minHeight = child.element.height;
      }
      // Store child widths then apply margins
      var childMinWidth = child.minWidth;
      var childMinHeight = child.minHeight;
      if (typeof child.element.marginLeft == "number")
      {
        childMinWidth += child.element.marginLeft;
      }
      if (typeof child.element.marginRight == "number")
      {
        childMinWidth += child.element.marginRight;
      }
      if (typeof child.element.marginTop == "number")
      {
        childMinHeight += child.element.marginTop;
      }
      if (typeof child.element.marginBottom == "number")
      {
        childMinHeight += child.element.marginBottom;
      }
      // Apply to parent
      if (parentMinWidth < childMinWidth)
      {
        parentMinWidth = childMinWidth;
      }
      if (parentMinHeight < childMinHeight)
      {
        parentMinHeight = childMinHeight;
      }
    });
    element.minWidth = parentMinWidth;
    element.minHeight = parentMinHeight;
  }

  measureElementBounds(element: LayoutElementWithBounds)
  {
    switch (element.element.elTyp)
    {
      case "Stack":
        this.measureStackElementBounds(element);
        break;
    }
  }

  measureStackElementBounds(element: LayoutElementWithBounds)
  {
    element.children.forEach((child: LayoutElementWithBounds) => {
      // Set from parent
      child.left = element.left;
      child.top = element.top;
      child.right = element.right;
      child.bottom = element.bottom;
      // Apply margins
      if (typeof child.element.marginLeft == "number")
      {
        child.left += child.element.marginLeft;
      }
      if (typeof child.element.marginRight == "number")
      {
        child.right -= child.element.marginRight;
      }
      if (typeof child.element.marginTop == "number")
      {
        child.top += child.element.marginTop;
      }
      if (typeof child.element.marginBottom == "number")
      {
        child.bottom -= child.element.marginBottom;
      }
      // Apply alignment
      if (typeof child.element.halign === "string")
      {
        switch (child.element.halign)
        {
          case "Left":
            child.right = child.left + child.minWidth;
            break;
          case "Centre":
            {
              var center = (child.left + child.right) / 2;
              child.left = center - child.minWidth / 2;
              child.right = center + child.minWidth / 2;
              break;
            }
          case "Right":
            child.left = child.right +- child.minWidth;
            break;
        }
      }
      if (typeof child.element.valign === "string")
      {
        switch (child.element.valign)
        {
          case "Top":
            child.bottom = child.top + child.minHeight;
            break;
          case "Centre":
            {
              var center = (child.top + child.bottom) / 2;
              child.top = center - child.minHeight / 2;
              child.bottom = center + child.minHeight / 2;
              break;
            }
          case "Bottom":
            child.top = child.bottom +- child.minHeight;
            break;
        }
      }
      // Layout child
      this.measureElementBounds(child);
    });
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
