import { ComponentFactoryResolver } from '@angular/core';
import { Console } from 'console';

import { LayoutData, LayoutFontConfig, LayoutAudioConfig, LayoutVideoResource, LayoutProperty } from '../layout-data';

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
  canvas: HTMLCanvasElement;
  context: CanvasRenderingContext2D;
  layoutData: LayoutData | undefined;
  rootElementWithBounds: LayoutElementWithBounds | undefined;

  propValues: any;

  loadingMessage: string | null = null;
  loadingBackColour: string | null = null;

  videoElementList: LayoutVideoResource[] = [];

  constructor(element: HTMLElement) {
    this.canvas = element as HTMLCanvasElement;
    this.context = this.canvas.getContext("2d")!;
  }

  setLayout(layoutData: LayoutData)
  {
    this.layoutData = layoutData;
    this.rootElementWithBounds = this.createElementWithBounds(layoutData, layoutData.rootElement);
  }

  setPropValues(propValues: any)
  {
    this.propValues = propValues;
  }

  updateOverlay()
  {
    // Update audios
    for (var audioIndex in this.layoutData!.audioConfigs) {
      var audioJson: LayoutAudioConfig = this.layoutData!.audioConfigs[audioIndex];
      var propValue: any = this.propValues[audioJson.propIndex];
      switch (propValue.State)
      {
        case "Stopped":
          if (audioJson.playing)
          {
            audioJson.audioRes.audioElement!.pause();
            audioJson.playing = false;
          }
          if (audioJson.stopped === false)
          {
            audioJson.audioRes.audioElement!.currentTime = 0;
            audioJson.stopped = true;
          }
          break;
        case "Paused":
          if (audioJson.playing)
          {
            audioJson.audioRes.audioElement!.pause();
            audioJson.playing = false;
          }
          break;
        case "Playing":
          if (audioJson.playing === false)
          {
            audioJson.audioRes.audioElement!.play();
            audioJson.playing = true;
            audioJson.stopped = false;
          }
          break;
      }
    }
    // Update videos
    for (var videoIndex in this.layoutData!.videos) {
      var videoResource: LayoutVideoResource = this.layoutData!.videos[videoIndex];
      var propValue: any = this.propValues[videoResource.propIndex];
      var isPlaying: boolean = videoResource!.videoElement!.paused === false && videoResource!.videoElement!.ended === false;
      var shouldBePlaying: boolean = propValue.State === "Playing";
      if (shouldBePlaying && propValue.PlayCount !== videoResource.currentPlayCount) {
        videoResource.videoElement!.currentTime = 0;
        videoResource.videoElement!.play();
        videoResource.videoElement!.style.display = "inline";
        videoResource.currentPlayCount = propValue.PlayCount;
      }
      if (shouldBePlaying === false && isPlaying) {
        videoResource.videoElement!.style.display = "none";
        videoResource.videoElement!.pause();
        videoResource.videoElement!.currentTime = 0;
      }
    }
  }

  createElementWithBounds(layoutData: LayoutData, element: any): LayoutElementWithBounds
  {
    var elementWithBounds: LayoutElementWithBounds = new LayoutElementWithBounds();
    elementWithBounds.element = element;
    switch (element.elTyp)
    {
      case "HFloat":
        if (element.left !== undefined)
          elementWithBounds.children.push(this.createElementWithBounds(layoutData, element.left));
        if (element.middle !== undefined)
          elementWithBounds.children.push(this.createElementWithBounds(layoutData, element.middle));
        if (element.right !== undefined)
          elementWithBounds.children.push(this.createElementWithBounds(layoutData, element.right));
        break;
      case "VFloat":
        if (element.top !== undefined)
          elementWithBounds.children.push(this.createElementWithBounds(layoutData, element.top));
        if (element.middle !== undefined)
          elementWithBounds.children.push(this.createElementWithBounds(layoutData, element.middle));
        if (element.bottom !== undefined)
          elementWithBounds.children.push(this.createElementWithBounds(layoutData, element.bottom));
        break;
      default:
        element.children?.forEach((item: any) => {
          elementWithBounds.children.push(this.createElementWithBounds(layoutData, item));
        });
        break;
    }
    return elementWithBounds;
  }

  getString(layoutData: LayoutData, object: any) : any {
    var noFormatText = this.getStringNoFormat(layoutData, object.text);
    if (object.format !== undefined)
    {
      switch (object.format)
      {
        case "Integer":
          return Number(noFormatText).toLocaleString('en-GB');
      }
    }
    return noFormatText;
  }

  getStringNoFormat(layoutData: LayoutData, object: any) : any {
    if (typeof object === "string")
    {
      return object;
    }
    if (object.op === "Text")
    {
      return object.text;
    }
    if (object.op === "Prop")
    {
      return this.getPropString(this.layoutData!.properties[object.index], this.propValues[object.index]);
    }
    if (object.op === "Cat")
    {
      var result = "";
      for (var itemIndex in object.items) {
        result += this.getStringNoFormat(layoutData, object.items[itemIndex]);
      }
      return result;
    }
    if (object.op === "If")
    {
      if (this.checkCondition(object.if, false))
        return this.getStringNoFormat(layoutData, object.then);
      else
        return this.getStringNoFormat(layoutData, object.else);
    }
    if (object.op === "IntegerToString")
    {
      var intResult: number = this.getInteger(object.integer);
      switch (object.format)
      {
        case "MinsAndSecs":
          {
            let str = '';
            str += (Math.floor(intResult / 600) % 6);
            str += (Math.floor(intResult / 60) % 10);
            str += ':';
            str += (Math.floor(intResult / 10) % 6);
            str += (intResult % 10);
            return str;
          }
      }
      return new String(intResult);
    }
    return object;
  }

  getPropString(propDef: LayoutProperty, value: any) : any {
    if (propDef.type == "Timer")
    {
      var valueSecs : number = value.Secs;
      if (value.StartDateTime !== undefined)
      {
        // Adjust for running timer
        var startDateTime = new Date(value.StartDateTime);
        var curDateTime = new Date();
        var deltaMillis = curDateTime.getTime() - startDateTime.getTime();
        var deltaSecs = Math.floor(deltaMillis / 1000);
        valueSecs = valueSecs + propDef.propDef.direction * deltaSecs;
        // Clamp to limits
        if (propDef.propDef.minValue !== undefined && valueSecs < propDef.propDef.minValue)
          valueSecs = propDef.propDef.minValue;
        if (propDef.propDef.maxValue !== undefined && propDef.propDef.maxValue < valueSecs)
          valueSecs = propDef.propDef.maxValue;
      }
      var date = new Date(2000, 1, 1, 0, 0, valueSecs, 0);
      return date.toLocaleTimeString();
    }
    return value;
  }

  getPropInteger(propIndex: number, propDef: LayoutProperty, value: any) : any {
    if (propDef.type == "Timer")
    {
      var valueSecs : number = value.Secs;
      if (value.StartDateTime !== undefined)
      {
        // Adjust for running timer
        var startDateTime = new Date(value.StartDateTime);
        var curDateTime = new Date();
        var deltaMillis = curDateTime.getTime() - startDateTime.getTime();
        var deltaSecs = Math.floor(deltaMillis / 1000);
        valueSecs = valueSecs + propDef.propDef.direction * deltaSecs;
        // Clamp to limits
        if (propDef.propDef.minValue !== undefined && valueSecs < propDef.propDef.minValue)
          valueSecs = propDef.propDef.minValue;
        if (propDef.propDef.maxValue !== undefined && propDef.propDef.maxValue < valueSecs)
          valueSecs = propDef.propDef.maxValue;
      }
      return valueSecs;
    }
    if (propDef.type == "Audio")
    {
      var audioConfig : LayoutAudioConfig = this.layoutData!.audioConfigs[propIndex];
      return Math.floor(audioConfig.audioRes.audioElement?.currentTime!);
    }
    return value;
  }

  checkCondition(condition: any, defaultValue: boolean) : boolean {
    if (condition === undefined || condition == null)
    {
      return defaultValue;
    }
    if (condition.op === "Prop")
    {
      return this.propValues[condition.index];
    }
    if (condition.op === "Equals")
    {
      return this.getStringNoFormat(this.layoutData!, condition.lhs) === this.getStringNoFormat(this.layoutData!, condition.rhs);
    }
    if (condition.op === "And")
    {
      for (var itemIndex in condition.items) {
        if (this.checkCondition(condition.items[itemIndex], false) === false)
          return false;
      }
      return true;
    }
    if (condition.op === "Or")
    {
      for (var itemIndex in condition.items) {
        if (this.checkCondition(condition.items[itemIndex], false))
          return true;
      }
      return false;
    }
    if (condition.op === "Not")
    {
      return !this.checkCondition(condition.item, false);
    }
    if (condition.op === "IntLessThan" || condition.op === "IntLessThan")
    {
      return this.getInteger(condition.lhs) < this.getInteger(condition.rhs);
    }
    if (condition.op === "IntLessThanEquals" || condition.op === "IntLessOrEqual")
    {
      return this.getInteger(condition.lhs) <= this.getInteger(condition.rhs);
    }
    if (condition.op === "IntGreaterThan")
    {
      return this.getInteger(condition.lhs) > this.getInteger(condition.rhs);
    }
    if (condition.op === "IntGreaterThanEquals")
    {
      return this.getInteger(condition.lhs) >= this.getInteger(condition.rhs);
    }
    return defaultValue;
  }

  getInteger(integerObj: any): number
  {
    if (integerObj.op === "Value")
    {
      return integerObj.value;
    }
    if (integerObj.op === "Prop")
    {
      return this.getPropInteger(integerObj.index, this.layoutData!.properties[integerObj.index], this.propValues[integerObj.index]);
    }
    if (integerObj.op === "Add")
    {
      return this.getInteger(integerObj.lhs) + this.getInteger(integerObj.rhs);
    }
    if (integerObj.op === "Subtract" || integerObj.op === "Sub")
    {
      return this.getInteger(integerObj.lhs) - this.getInteger(integerObj.rhs);
    }
    if (integerObj.op === "Multiply")
    {
      return this.getInteger(integerObj.lhs) * this.getInteger(integerObj.rhs);
    }
    if (integerObj.op === "Divide")
    {
      return this.getInteger(integerObj.lhs) / this.getInteger(integerObj.rhs);
    }
    if (integerObj.op === "Modulo")
    {
      return this.getInteger(integerObj.lhs) % this.getInteger(integerObj.rhs);
    }
    if (integerObj.op === "AudioPositionSecs")
    {
      var audioConfig : LayoutAudioConfig = this.layoutData!.audioConfigs[integerObj.audio];
      return Math.floor(audioConfig.audioRes.audioElement?.currentTime!);
    }
    if (integerObj.op === "AudioDurationSecs")
    {
      var audioConfig : LayoutAudioConfig = this.layoutData!.audioConfigs[integerObj.audio];
      return Math.floor(audioConfig.audioRes.audioElement?.duration!);
    }
    return 0;
  }

  setTransform(actWidth: number, actHeight: number)
  {
    var canWidth: number = this.layoutData!.canvasSize[0];
    var canHeight: number = this.layoutData!.canvasSize[1];
    var widthActOverCan: number = actWidth / canWidth;
    var heightActOverCan: number = actHeight / canHeight;
    if (widthActOverCan === heightActOverCan)
    {
      this.context.scale(widthActOverCan, widthActOverCan);
    }
    else if (widthActOverCan < heightActOverCan)
    {
      this.context.scale(widthActOverCan, widthActOverCan);
      this.context.translate(0.0, (actHeight - widthActOverCan * canHeight) / (2.0 * widthActOverCan));
    }
    else
    {
      this.context.scale(heightActOverCan, heightActOverCan);
      this.context.translate((actWidth - heightActOverCan * canWidth) / (2.0 * heightActOverCan), 0.0);
    }
  }

  measureRootElementWithBounds()
  {
    this.rootElementWithBounds!.left = 0;
    this.rootElementWithBounds!.top = 0;
    this.rootElementWithBounds!.right = this.layoutData!.canvasSize[0];
    this.rootElementWithBounds!.bottom = this.layoutData!.canvasSize[1];
    this.measureElementMinimumSize(this.rootElementWithBounds!);
    this.measureElementBounds(this.rootElementWithBounds!);
  }

  measureElementMinimumSize(element: LayoutElementWithBounds)
  {
    switch (element.element.elTyp)
    {
      case "ColouredQuad":
        this.measureColouredQuadElementMinimumSize(element);
        break;
      case "HFloat":
        this.measureHFloatElementMinimumSize(element);
        break;
      case "Image":
        this.measureImageElementMinimumSize(element);
        break;
      case "Rows":
        this.measureRowsElementMinimumSize(element);
        break;
      case "Stack":
      case "Transform":
        this.measureStackElementMinimumSize(element);
        break;
      case "Text":
        this.measureTextElementMinimumSize(element);
        break;
      case "VFloat":
        this.measureVFloatElementMinimumSize(element);
        break;
    }
  }

  measureColouredQuadElementMinimumSize(elementWithBounds: LayoutElementWithBounds)
  {
    if (typeof elementWithBounds.element.width == "number")
      elementWithBounds.minWidth = elementWithBounds.element.width;
    else
      elementWithBounds.minWidth = 0;
    if (typeof elementWithBounds.element.height == "number")
      elementWithBounds.minHeight = elementWithBounds.element.height;
    else
      elementWithBounds.minHeight = 0;
  }

  measureHFloatElementMinimumSize(element: LayoutElementWithBounds)
  {
    var parentMinWidth = 0;
    var parentMinHeight = 0;
    // Apply children
    element.children.forEach((child: LayoutElementWithBounds) => {
      this.measureElementMinimumSize(child);
      parentMinWidth += child.minWidth;
      if (parentMinHeight < child.minHeight)
        parentMinHeight = child.minHeight;
    });
    // Apply spacing
    if (typeof element.element.spacing == "number" && 1 < element.children.length)
      parentMinWidth = element.element.spacing * (element.children.length - 1);
    // Apply margins
    if (typeof element.element.marginLeft == "number")
      parentMinWidth += element.element.marginLeft;
    if (typeof element.element.marginRight == "number")
      parentMinWidth += element.element.marginRight;
    if (typeof element.element.marginTop == "number")
      parentMinHeight += element.element.marginTop;
    if (typeof element.element.marginBottom == "number")
      parentMinHeight += element.element.marginBottom;
    // Set measurements
    element.minWidth = parentMinWidth;
    element.minHeight = parentMinHeight;
  }

  measureImageElementMinimumSize(elementWithBounds: LayoutElementWithBounds)
  {
    var minWidth = 0;
    var minHeight = 0;
    if (elementWithBounds.element.hfit === "None" || elementWithBounds.element.vfit === "None")
    {
      var image: HTMLImageElement = this.layoutData!.imageResources[elementWithBounds.element.res]!.imageElement!;
      if (elementWithBounds.element.hfit === "None")
      {
        minWidth = image.width;
      }
      if (elementWithBounds.element.vfit === "None")
      {
        minHeight = image.height;
      }
    }
    elementWithBounds.minWidth = minWidth;
    elementWithBounds.minHeight = minHeight;
  }

  measureRowsElementMinimumSize(element: LayoutElementWithBounds)
  {
    var parentMinWidth = 0;
    var parentMinHeight = 0;
    var isFirst: boolean = true;
    element.children.forEach((child: LayoutElementWithBounds) => {
      this.measureElementMinimumSize(child);
      // Apply spacing
      if (isFirst)
      {
        isFirst = false;
      }
      else if (typeof element.element.spacing == "number")
      {
        parentMinHeight += element.element.spacing;
      }
      // Apply widths and heights
      if (parentMinWidth < child.minWidth)
        parentMinWidth = child.minWidth;
      parentMinHeight += child.minHeight;
    });
    element.minWidth = parentMinWidth;
    element.minHeight = parentMinHeight;
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
      if (typeof child.element.stackMarginLeft == "number")
        childMinWidth += child.element.stackMarginLeft;
      if (typeof child.element.stackMarginRight == "number")
        childMinWidth += child.element.stackMarginRight;
      if (typeof child.element.stackMarginTop == "number")
        childMinHeight += child.element.stackMarginTop;
      if (typeof child.element.stackMarginBottom == "number")
        childMinHeight += child.element.stackMarginBottom;
      // Apply to parent
      if (parentMinWidth < childMinWidth)
        parentMinWidth = childMinWidth;
      if (parentMinHeight < childMinHeight)
        parentMinHeight = childMinHeight;
    });
    element.minWidth = parentMinWidth;
    element.minHeight = parentMinHeight;
  }

  measureTextElementMinimumSize(element: LayoutElementWithBounds)
  {
    var fontConfig: LayoutFontConfig = this.layoutData!.fontConfigs[element.element.font];
    this.context!.font = fontConfig.fontName;
    var metrics: TextMetrics = this.context!.measureText(element.element.text);
    element.minWidth = metrics.width;
    element.minHeight = metrics.actualBoundingBoxAscent + metrics.actualBoundingBoxDescent;
  }

  measureVFloatElementMinimumSize(element: LayoutElementWithBounds)
  {
    var parentMinWidth = 0;
    var parentMinHeight = 0;
    // Apply children
    element.children.forEach((child: LayoutElementWithBounds) => {
      this.measureElementMinimumSize(child);
      if (parentMinWidth < child.minWidth)
        parentMinWidth = child.minWidth;
      parentMinHeight += child.minHeight;
    });
    // Apply spacing
    if (typeof element.element.spacing == "number" && 1 < element.children.length)
      parentMinHeight = element.element.spacing * (element.children.length - 1);
    // Apply margins
    if (typeof element.element.marginLeft == "number")
      parentMinWidth += element.element.marginLeft;
    if (typeof element.element.marginRight == "number")
      parentMinWidth += element.element.marginRight;
    if (typeof element.element.marginTop == "number")
      parentMinHeight += element.element.marginTop;
    if (typeof element.element.marginBottom == "number")
      parentMinHeight += element.element.marginBottom;
    // Set measurements
    element.minWidth = parentMinWidth;
    element.minHeight = parentMinHeight;
  }

  measureElementBounds(element: LayoutElementWithBounds)
  {
    switch (element.element.elTyp)
    {
      case "ColouredQuad":
        this.measureColouredQuadElementBounds(element);
        break;
      case "HFloat":
        this.measureHFloatElementBounds(element);
        break;
      case "Rows":
        this.measureRowsElementBounds(element);
        break;
      case "Stack":
      case "Transform":
        this.measureStackElementBounds(element);
        break;
      case "VFloat":
        this.measureVFloatElementBounds(element);
        break;
    }
  }

  measureColouredQuadElementBounds(element: LayoutElementWithBounds)
  {
    if (typeof element.element.width == "number")
    {
      var oldWidth: number = element.right - element.left;
      var newWidth: number = element.element.width;
      if (newWidth < oldWidth)
        element.right = element.left + newWidth;
    }
    if (typeof element.element.height == "number")
    {
      var oldHeight: number = element.bottom - element.top;
      var newHeight: number = element.element.height;
      if (newHeight < oldHeight)
        element.bottom = element.top + newHeight;
    }
  }

  measureHFloatElementBounds(element: LayoutElementWithBounds)
  {
    // Work out child indices
    var leftIndex: number = -1;
    var middleIndex: number = -1;
    var rightIndex: number = -1;
    if (element.element.left !== undefined)
      leftIndex = 0;
    if (element.element.middle !== undefined)
      middleIndex = leftIndex + 1;
    if (element.element.right !== undefined)
      rightIndex = (middleIndex === -1) ? (leftIndex + 1) : (middleIndex + 1);
    // Get bounds
    var parentLeft: number = element.left;
    var parentRight: number = element.right;
    var parentTop: number = element.top;
    var parentBottom: number = element.bottom;
    // Apply margins
    if (typeof element.element.marginLeft == "number")
      parentLeft += element.element.marginLeft;
    if (typeof element.element.marginRight == "number")
      parentRight -= element.element.marginRight;
    if (typeof element.element.marginTop == "number")
      parentTop += element.element.marginTop;
    if (typeof element.element.marginBottom == "number")
      parentBottom -= element.element.marginBottom;
    // Apply top element
    if (0 <= leftIndex)
    {
      var child: LayoutElementWithBounds = element.children[leftIndex];
      child.left = parentLeft;
      parentLeft += child.minWidth;
      child.right = parentLeft;
      child.top = parentTop;
      child.bottom = parentBottom;
    }
    // Apply right element
    if (0 <= rightIndex)
    {
      var child: LayoutElementWithBounds = element.children[rightIndex];
      child.right = parentRight;
      parentRight -= child.minWidth;
      child.left = parentRight;
      child.top = parentTop;
      child.bottom = parentBottom;
    }
    // Apply spacing
    if (typeof element.element.spacing == "number")
    {
      if (0 <= leftIndex)
        parentLeft += element.element.spacing;
      if (0 <= rightIndex)
        parentRight -= element.element.spacing;
    }
    // Apply middle
    if (0 <= middleIndex)
    {
      var child: LayoutElementWithBounds = element.children[middleIndex];
      child.left = parentLeft;
      child.right = parentRight;
      child.top = parentTop;
      child.bottom = parentBottom;
    }
    // Traverse down
    if (0 <= leftIndex)
      this.measureElementBounds(element.children[leftIndex]);
    if (0 <= middleIndex)
      this.measureElementBounds(element.children[middleIndex]);
    if (0 <= rightIndex)
      this.measureElementBounds(element.children[rightIndex]);
  }

  measureRowsElementBounds(element: LayoutElementWithBounds)
  {
    var curTop: number = element.top;
    var isFirst: boolean = true;
    element.children.forEach((child: LayoutElementWithBounds) => {
      // Apply spacing
      if (isFirst)
        isFirst = false;
      else if (typeof element.element.spacing == "number")
        curTop += element.element.spacing;
      // Set from parent
      child.left = element.left;
      child.top = curTop;
      child.right = element.right;
      child.bottom = curTop + child.minHeight;
      curTop = child.bottom;
      // Layout child
      this.measureElementBounds(child);
    });
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
      if (typeof child.element.stackMarginLeft == "number")
        child.left += child.element.stackMarginLeft;
      if (typeof child.element.stackMarginRight == "number")
        child.right -= child.element.stackMarginRight;
      if (typeof child.element.stackMarginTop == "number")
        child.top += child.element.stackMarginTop;
      if (typeof child.element.stackMarginBottom == "number")
        child.bottom -= child.element.stackMarginBottom;
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

  measureVFloatElementBounds(element: LayoutElementWithBounds)
  {
    // Work out child indices
    var topIndex: number = -1;
    var middleIndex: number = -1;
    var bottomIndex: number = -1;
    if (element.element.top !== undefined)
      topIndex = 0;
    if (element.element.middle !== undefined)
      middleIndex = topIndex + 1;
    if (element.element.bottom !== undefined)
      bottomIndex = (middleIndex === -1) ? (topIndex + 1) : (middleIndex + 1);
    // Get bounds
    var parentLeft: number = element.left;
    var parentRight: number = element.right;
    var parentTop: number = element.top;
    var parentBottom: number = element.bottom;
    // Apply margins
    if (typeof element.element.marginLeft == "number")
      parentLeft += element.element.marginLeft;
    if (typeof element.element.marginRight == "number")
      parentRight -= element.element.marginRight;
    if (typeof element.element.marginTop == "number")
      parentTop += element.element.marginTop;
    if (typeof element.element.marginBottom == "number")
      parentBottom -= element.element.marginBottom;
    // Apply top element
    if (0 <= topIndex)
    {
      var child: LayoutElementWithBounds = element.children[topIndex];
      child.left = parentLeft;
      child.right = parentRight;
      child.top = parentTop;
      parentTop += child.minHeight;
      child.bottom = parentTop;
    }
    // Apply top element
    if (0 <= bottomIndex)
    {
      var child: LayoutElementWithBounds = element.children[bottomIndex];
      child.left = parentLeft;
      child.right = parentRight;
      child.bottom = parentBottom;
      parentBottom -= child.minHeight;
      child.top = parentBottom;
    }
    // Apply spacing
    if (typeof element.element.spacing == "number")
    {
      if (0 <= topIndex)
        parentTop += element.element.spacing;
      if (0 <= bottomIndex)
        parentBottom -= element.element.spacing;
    }
    // Apply middle
    if (0 <= middleIndex)
    {
      var child: LayoutElementWithBounds = element.children[middleIndex];
      child.left = parentLeft;
      child.right = parentRight;
      child.top = parentTop;
      child.bottom = parentBottom;
    }
    // Traverse down
    if (0 <= topIndex)
      this.measureElementBounds(element.children[topIndex]);
    if (0 <= middleIndex)
      this.measureElementBounds(element.children[middleIndex]);
    if (0 <= bottomIndex)
      this.measureElementBounds(element.children[bottomIndex]);
  }

  render(isLoaded: boolean) : void
  {
    // Sizes
    var width = this.canvas.width;
    var height = this.canvas.height;

    // Clear
    this.context.resetTransform();
    this.context!.clearRect(0, 0, width, height);

    // Render
    if (isLoaded === false || this.rootElementWithBounds === undefined || this.layoutData === undefined)
    {
      this.renderLoading(width, height);
    }
    else
    {
      this.setTransform(width, height);
      this.measureRootElementWithBounds();
      this.renderElement(this.rootElementWithBounds!);
    }
  }

  renderLoading(width: number, height: number)
  {
    if (this.loadingBackColour !== null)
    {
      this.context.fillStyle = this.loadingBackColour;
      this.context.fillRect(0, 0, width, height);
    }
    if (this.loadingMessage !== null)
    {
      this.context.fillStyle = '#FFFFFF';
      this.context.font = '28px Helvetica';
      this.context.fillText(this.loadingMessage, 20, 50);
    }
  }

  renderElement(elementWithBounds: LayoutElementWithBounds)
  {
    // Check we need to render
    if (this.checkCondition(elementWithBounds.element.isVisible, true) == false)
    {
      return;
    }

    // Render depending on type
    switch (elementWithBounds.element.elTyp)
    {
      // Layouts
      case "HFloat":
      case "Rows":
      case "Stack":
      case "VFloat":
        this.renderElementStack(elementWithBounds);
        break;
      // Elements
      case "ColouredQuad":
        this.renderElementColouredQuad(elementWithBounds);
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
      case "Transform":
        this.renderTransformElement(elementWithBounds);
        break;
      case "Video":
        this.renderVideoElement(elementWithBounds);
        break;
    }
  }

  renderElementStack(elementWithBounds: LayoutElementWithBounds)
  {
    elementWithBounds.children.forEach((item: any) => {
      this.renderElement(item);
    });
  }

  renderElementColouredQuad(elementWithBounds: LayoutElementWithBounds)
  {
    var width : number = elementWithBounds.right - elementWithBounds.left;
    var height : number = elementWithBounds.bottom - elementWithBounds.top;
    this.context!.fillStyle = elementWithBounds.element.hex;
    this.context!.fillRect(elementWithBounds.left, elementWithBounds.top, width, height);
  }

  renderElementImage(elementWithBounds: LayoutElementWithBounds)
  {
    var image: HTMLImageElement = this.layoutData!.imageResources[elementWithBounds.element.res]!.imageElement!;
    switch (elementWithBounds.element.hfit)
    {
      case "None":
        this.renderElementImageSimpleHFit(image, elementWithBounds);
        break;
      case "Tiled":
        this.renderElementImageTiledHFit(image, elementWithBounds);
        break;
      case "Scaled":
        this.renderElementImageScaledHFit(image, elementWithBounds);
        break;
    }
  }

  renderElementImageSimpleHFit(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds)
  {
    var width : number = elementWithBounds.right - elementWithBounds.left;
    this.renderElementImageVStrip(image, elementWithBounds, elementWithBounds.left, width, width);
  }

  renderElementImageTiledHFit(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds)
  {
    let imageWidth = image.width;
    if (imageWidth <= 0)
    {
      return;
    }
    let curLeft = 0;
    var left : number = elementWithBounds.left;
    var width : number = elementWithBounds.right - elementWithBounds.left;
    while (curLeft < width)
    {
      let drawWidth = (width < curLeft + imageWidth) ? (width - curLeft) : imageWidth;
      this.renderElementImageVStrip(image, elementWithBounds, left + curLeft, drawWidth, drawWidth);
      curLeft += imageWidth;
    }
  }

  renderElementImageScaledHFit(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds)
  {
    var renderWidth : number = elementWithBounds.right - elementWithBounds.left;
    this.renderElementImageVStrip(image, elementWithBounds, elementWithBounds.left, renderWidth, image.width);
  }

  renderElementImageVStrip(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds, left: number, renderWidth: number, imageWidth: number)
  {
    switch (elementWithBounds.element.vfit)
    {
      case "None":
        this.renderElementImageSimpleVFit(image, elementWithBounds, left, renderWidth, imageWidth);
        break;
      case "Tiled":
        this.renderElementImageTiledVFit(image, elementWithBounds, left, renderWidth, imageWidth);
        break;
      case "Scaled":
        this.renderElementImageScaledVFit(image, elementWithBounds, left, renderWidth, imageWidth);
        break;
    }
  }

  renderElementImageSimpleVFit(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds, left: number, renderWidth: number, imageWidth: number)
  {
    var height : number = elementWithBounds.bottom - elementWithBounds.top;
    this.context!.drawImage(image, 0, 0, imageWidth, height, left, elementWithBounds.top, renderWidth, height);
  }

  renderElementImageTiledVFit(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds, left: number, renderWidth: number, imageWidth: number)
  {
    let imageHeight = image.height;
    if (imageHeight <= 0)
    {
      return;
    }
    let curTop = 0;
    var top : number = elementWithBounds.top;
    var height : number = elementWithBounds.bottom - elementWithBounds.top;
    while (curTop < height)
    {
      let drawHeight = (height < curTop + imageHeight) ? (height - curTop) : imageHeight;
      this.context!.drawImage(image, 0, 0, imageWidth, drawHeight, left, top + curTop, renderWidth, drawHeight);
      curTop += imageHeight;
    }
  }

  renderElementImageScaledVFit(image: HTMLImageElement, elementWithBounds: LayoutElementWithBounds, left: number, renderWidth: number, imageWidth: number)
  {
    var renderHeight : number = elementWithBounds.bottom - elementWithBounds.top;
    this.context!.drawImage(image, 0, 0, imageWidth, image.height, left, elementWithBounds.top, renderWidth, renderHeight);
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
    var fontConfig: LayoutFontConfig = this.layoutData!.fontConfigs[elementWithBounds.element.font];
    this.context!.fillStyle = fontConfig.fontJson.colour;
    this.context!.font = fontConfig.fontName;
    switch (fontConfig.fontJson.align)
    {
      case "Centre":
        this.context!.textAlign = 'center';
        this.context!.fillText(this.getString(this.layoutData!, elementWithBounds.element), (elementWithBounds.left + elementWithBounds.right) / 2, elementWithBounds.bottom);
        break;
      case "Right":
        this.context!.textAlign = 'right';
        this.context!.fillText(this.getString(this.layoutData!, elementWithBounds.element), elementWithBounds.right, elementWithBounds.bottom);
        break;
      default:
        this.context!.textAlign = 'left';
        this.context!.fillText(this.getString(this.layoutData!, elementWithBounds.element), elementWithBounds.left, elementWithBounds.bottom);
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

  renderTransformElement(elementWithBounds: LayoutElementWithBounds)
  {
    // Push transform and execute
    this.context.save();
    elementWithBounds.element.steps.forEach((step: any) => {
      switch (step.type)
      {
        case "rot":
          this.renderTransformRotateStep(elementWithBounds, step);
          break;
      }
    });

    // Render children
    this.renderElementStack(elementWithBounds);

    // Pop transform
    this.context.restore();
  }

  renderTransformRotateStep(elementWithBounds: LayoutElementWithBounds, step: any)
  {
    var pivotX : number = elementWithBounds.left;
    var pivotY : number = elementWithBounds.top;
    this.context.translate(pivotX, pivotY);
    this.context.rotate((step.deg_cw * Math.PI) / 180);
    this.context.translate(-pivotX, -pivotY);
  }

  renderVideoElement(elementWithBounds: LayoutElementWithBounds)
  {
    var videoIndex: number = Number(elementWithBounds.element.res);
    if (videoIndex < this.videoElementList.length)
    {
      var videoResource: LayoutVideoResource = this.videoElementList[videoIndex];
      var left : number = elementWithBounds.left;
      var top : number = elementWithBounds.top;
      var width : number = elementWithBounds.right - elementWithBounds.left;
      var height : number = elementWithBounds.bottom - elementWithBounds.top;
      videoResource.videoElement!.style.position = 'fixed';
      videoResource.videoElement!.style.left = left + 'px';
      videoResource.videoElement!.style.top = top + 'px';
      videoResource.videoElement!.style.width = width + 'px';
      videoResource.videoElement!.style.height = height + 'px';
    }
  }
}
