export class LayoutControlsApiDto
{
  Name: string | undefined;
  
  Fields: LayoutControlsFieldApiDto[] | undefined;
}

export class LayoutControlsFieldApiDto
{
  PropIndex: number | undefined;
  
  ValueType: string | undefined;
  
  Label: string | undefined;
  
  FieldDef: any[] | undefined;
  
  DefaultValue: any;
}
