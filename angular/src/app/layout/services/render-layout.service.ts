import { LayoutData } from '../layout-data';

enum RenderLayoutSizeType { Pixel, Min, Max }

class RenderLayoutSize {
  widthType: RenderLayoutSizeType = RenderLayoutSizeType.Pixel;
  width: number = 0;
  heightType: RenderLayoutSizeType = RenderLayoutSizeType.Pixel;
  height: number = 0;
}

export class RenderLayoutService {
  context: CanvasRenderingContext2D | undefined;

  constructor(context: CanvasRenderingContext2D) {
    this.context = context;
  }

  measureElement(layoutData: LayoutData, element: any): RenderLayoutSize
  {
    var size: RenderLayoutSize = this.measureElementNoMargin(layoutData, element);
    if (element.width !== undefined)
    {
      if (typeof element.width === "number") {
        size.width = element.width;
      }
      else if (element.width === "max") {
        size.widthType = RenderLayoutSizeType.Max;
      }
      else if (element.width === "min") {
        size.widthType = RenderLayoutSizeType.Min;
      }
    }
    if (element.height !== undefined)
    {
      if (typeof element.height === "number") {
        size.height = element.height;
      }
      else if (element.height === "max") {
        size.heightType = RenderLayoutSizeType.Max;
      }
      else if (element.height === "min") {
        size.heightType = RenderLayoutSizeType.Min;
      }
    }
    return size;
  }

  measureElementNoMargin(layoutData: LayoutData, element: any): RenderLayoutSize
  {
    switch (element.elTyp)
    {
      case "stack":
        return this.measureElementStack(layoutData, element);
      case "vsplit":
        return this.measureElementVSplit(layoutData, element);
      case "image":
        return this.measureElementImage(layoutData, element);
    }
    return new RenderLayoutSize();
  }

  measureElementStack(layoutData: LayoutData, element: any): RenderLayoutSize
  {
    var size: RenderLayoutSize = new RenderLayoutSize();
    element.children.forEach((item: any) => {
      var childSize = this.measureElement(layoutData, item);
      if (size.width < childSize.width) {
        size.width = childSize.width;
      }
      if (size.height < childSize.height) {
        size.height = childSize.height;
      }
    });
    return size;
  }

  measureElementVSplit(layoutData: LayoutData, element: any): RenderLayoutSize
  {
    // Get padding
    var padding: number = 0;
    if (typeof element.padding === "number") {
      padding = element.padding;
    }

    // Traverse children
    var size: RenderLayoutSize = new RenderLayoutSize();
    element.children.forEach((item: any) => {
      var childSize = this.measureElement(layoutData, item);
      if (size.width < childSize.width) {
        size.width = childSize.width;
      }
      size.height += childSize.height + padding;
    });
    size.width += padding + padding;
    size.height += padding;
    return size;
  }

  measureElementImage(layoutData: LayoutData, element: any): RenderLayoutSize
  {
    if (element.fit === "tiled") {
      return new RenderLayoutSize();
    }
    else {
      var image: HTMLImageElement = layoutData.imageResources.get(element.res)!.imageElement!;
      var size: RenderLayoutSize = new RenderLayoutSize();
      size.width = image.width;
      size.height = image.height;
      return size;
    }
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
      this.renderElement(layoutData, layoutData.rootElement, 0, 0, canvasWidth, canvasHeight);
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

  renderElement(layoutData: LayoutData, element: any, left: number, top: number, width: number, height: number)
  {
    // Handle horizontal alignment
    if (element.width === "min" || element.width === "max")
    {
      var size = this.measureElement(layoutData, element);
      if (element.width === "min")
      {
        element.width = size.width;
      }
      if (element.width === "max")
      {
        element.height = size.height;
      }
    }
    if (typeof element.halign === "string" && typeof element.width === "number")
    {
      switch (element.halign)
      {
        case "left":
          width = element.width;
          break;
        case "centre":
          width = element.width;
          break;
        case "right":
          {
            var newLeft = left + width - element.width;
            newLeft = (newLeft < left) ? left : newLeft;
            left = newLeft;
            width = element.width;
          }
          break;
      }
    }

    // Handle type
    switch (element.elTyp)
    {
      case "stack":
        this.renderElementStack(layoutData, element, left, top, width, height);
        break;
      case "vsplit":
        this.renderElementVSplit(layoutData, element, left, top, width, height);
        break;
      case "image":
        this.renderElementImage(layoutData, element, left, top, width, height);
        break;
      case "colour":
        this.renderElementColour(element, left, top, width, height);
        break;
    }
  }

  renderElementStack(layoutData: LayoutData, element: any, left: number, top: number, width: number, height: number)
  {
    element.children.forEach((item: any) => {
      this.renderElement(layoutData, item, left, top, width, height);
    });
  }

  renderElementVSplit(layoutData: LayoutData, element: any, left: number, top: number, width: number, height: number)
  {
    // Apply padding
    var padding: number = 0;
    if (typeof element.padding === "number") {
      padding = element.padding;
    }
    left += padding;
    top += padding;
    width -= padding + padding;
    height -= padding + padding;

    // Work out heights
    var heightLeft: number = height - padding * (element.children.length - 1);
    var numMaxChildren: number = 0;
    var childrenWithSizes: any[] = [];
    element.children.forEach((item: any) => {
      var childSize: RenderLayoutSize = this.measureElement(layoutData, item);
      if (childSize.heightType === RenderLayoutSizeType.Max)
      {
        ++numMaxChildren;
      }
      else
      {
        heightLeft -= childSize.height;
      }
      childrenWithSizes.push({child:item, size:childSize});
    });

    // Allocate extra heights
    if (0 < numMaxChildren)
    {
      var maxChildHeight = heightLeft / numMaxChildren;
      for(let childIndex = 0; childIndex < childrenWithSizes.length ; ++childIndex) {
        var childSize = childrenWithSizes[childIndex].size;
        if (childSize.heightType === RenderLayoutSizeType.Max) {
          childSize.height = maxChildHeight;
        }
      }
    }

    // Render
    for(let childIndex = 0; childIndex < childrenWithSizes.length ; ++childIndex) {
      var child = childrenWithSizes[childIndex];
      this.renderElement(layoutData, child.child, left, top, width, child.size.height);
      top += child.size.height + padding;
    }
  }

  renderElementImage(layoutData: LayoutData, element: any, left: number, top: number, width: number, height: number)
  {
    var image: HTMLImageElement = layoutData.imageResources.get(element.res)!.imageElement!;
    if (element.fit === "tiled") {
      this.renderElementImageTiled(image, left, top, width, height);
    }
    else {
      this.renderElementImageSimple(image, left, top, width, height);
    }
  }

  renderElementImageSimple(image: HTMLImageElement, left: number, top: number, width: number, height: number)
  {
    this.context!.drawImage(image, 0, 0, width, height, left, top, width, height);
  }

  renderElementImageTiled(image: HTMLImageElement, left: number, top: number, width: number, height: number)
  {
    let imageWidth = image.width;
    let imageHeight = image.height;
    if (imageWidth <= 0 || imageHeight <= 0)
    {
      return;
    }
    let curTop = 0;
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

  renderElementColour(element: any, left: number, top: number, width: number, height: number)
  {
    this.context!.fillStyle = element.colour;
    this.context!.fillRect(left, top, width, height);
  }
}
