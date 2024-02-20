export class EditLayoutModel {
  fullJson: any | null = null;

  layoutType: string | null = null;

  contentString: string | null = null;

  constructor(loadedJson: any | null, previousLayoutData: EditLayoutModel | null, contentString: string | null)
  {
    if (loadedJson !== null)
    {
        this.fullJson = loadedJson;
        this.layoutType = loadedJson.LayoutType;
        this.contentString = String(loadedJson.Layout);
    }
    else if (previousLayoutData != null)
    {
        this.fullJson = previousLayoutData.fullJson;
        this.layoutType = previousLayoutData.layoutType;
        if (contentString != null)
        {
            this.contentString = contentString;
        }
        else
        {
            this.contentString = previousLayoutData.contentString;
        }
    }
    else if (contentString != null)
    {
        this.layoutType = "RawXml";
        this.contentString = contentString;
    }
  }

  isEqual(other: EditLayoutModel) : boolean
  {
    return this.layoutType == other.layoutType && this.contentString == other.contentString;
  }

  createJsonObject() : any
  {
    var finalJson : any = this.fullJson;
    if (finalJson == null)
    {
        finalJson = { LayoutType: this.layoutType, RedrawMillis: 250  };
    }
    finalJson.Layout = this.contentString;
    return finalJson;
  }

  createJsonString() : string
  {
    return JSON.stringify(this.createJsonObject());
  }
}
