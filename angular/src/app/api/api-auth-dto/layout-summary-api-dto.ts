export class LayoutSummaryApiDto
{
  CreatedDateTime: string = "";
  
  LayoutId: string = "";
  
  Name: string = "";
  
  HasDraft: boolean = false;
  
  HasRelease: boolean = false;

  ControlsNameArray: string[] = [];
}
