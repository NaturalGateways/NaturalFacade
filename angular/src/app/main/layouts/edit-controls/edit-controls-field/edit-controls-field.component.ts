import { JsonPipe } from '@angular/common';
import { Component, Input } from '@angular/core';

import { MenuItem } from 'primeng/api';
import { ContextMenu } from 'primeng/contextmenu';

import { ApiService } from '../../../../api/api.service';

import { EditControlsField } from '../edit-controls-model';

@Component({
  selector: 'app-edit-controls-field',
  templateUrl: './edit-controls-field.component.html',
  styleUrls: ['./edit-controls-field.component.css']
})
export class EditControlsFieldComponent {
  @Input() layoutId : string | undefined;
  @Input() field : EditControlsField | undefined;

  isSaving: boolean = false;

  fieldLabel : string | undefined;
  savedStringValue : string | undefined;
  savedNumberValue : number | undefined;
  savedBoolValue : boolean = false;

  switchVisible : boolean = false;
  switchFalseLabel : string = "False";
  switchTrueLabel : string = "True";
  switchValue : boolean = false;

  integerVisible : boolean = false;
  integerValue : number = 0;
  integerMinValue : number = 0;
  integerMaxValue : number = 0;

  selectOptionsVisible : boolean = false;
  selectOptionsModel : MenuItem[] = [];

  textFieldVisible : boolean = false;
  textFieldText : string | undefined;

  constructor(private apiService: ApiService)
  {
    //
  }

  ngOnInit(): void {
    this.fieldLabel = this.field!.label;

    // Setup switch
    if (this.field!.fieldDef.Switch !== undefined && this.field!.fieldDef.Switch !== null)
    {
      this.switchVisible = true;
      var switchObj = this.field!.fieldDef.Switch;
      if (switchObj.FalseLabel !== undefined)
        this.switchFalseLabel = switchObj.FalseLabel;
      if (switchObj.TrueLabel !== undefined)
        this.switchTrueLabel = switchObj.TrueLabel;
    }

    // Setup numeric
    if (this.field!.fieldDef.Integer !== undefined)
    {
      this.integerVisible = true;
      var integerObj = this.field!.fieldDef.Integer;
      if (integerObj.MinValue !== undefined)
        this.integerMinValue = integerObj.MinValue;
      else
        this.integerMinValue = Number.MIN_VALUE;
      if (integerObj.MaxValue !== undefined)
        this.integerMaxValue = integerObj.MaxValue;
      else
        this.integerMaxValue = Number.MAX_VALUE;
    }

    // Setup choices
    if (this.field!.fieldDef.SelectOptions !== undefined)
    {
      this.selectOptionsVisible = true;
      var optionValues : string[] = this.field!.fieldDef.SelectOptions;
      optionValues.forEach(optionValue => {
        var controlsMenuItems : MenuItem =
        {
          label: optionValue,
          icon:'pi pi-fw pi-stop',
          command: e => this.upload(optionValue)
        };
        this.selectOptionsModel.push(controlsMenuItems);
      });
    }

    // Setup free text
    if (this.field!.fieldDef.TextField !== undefined)
    {
      this.textFieldVisible = true;
    }

    // Set value
    this.setSavedValue(this.field!.value);
  }

  setSavedValue(value: any)
  {
    switch (this.field!.valueType)
    {
      case "String":
        this.savedStringValue = value;
        this.textFieldText = value;
        if (this.field!.fieldDef.Integer !== undefined)
        {
          this.savedNumberValue = Number(value);
          this.integerValue = this.savedNumberValue;
        }
        break;
      case "Boolean":
        this.savedBoolValue = Boolean(value);
        this.switchValue = this.savedBoolValue;
        this.savedStringValue = this.savedBoolValue ? this.switchTrueLabel : this.switchFalseLabel;
        break;
    }
  }

  onSwitchToggled(event: any)
  {
    this.upload(event.checked);
  }

  onIntegerChanged() {
    this.upload(this.integerValue);
  }

  onNumericClear() {
    this.upload(this.field!.defaultValue);
  }

  onShowContextMenu(event: MouseEvent, contextMenu : ContextMenu)
  {
    // Show menu
    contextMenu.show(event);
    event.stopPropagation();
  }

  onSaveTextField()
  {
    this.upload(this.textFieldText!);
  }

  upload(value: any)
  {
    this.isSaving = true;
    var propIndex: number = this.field!.propIndex!;
    this.apiService.putLayoutPropertyValue(this.layoutId!, propIndex, value, () =>
    {
      this.setSavedValue(value);
      this.isSaving = false;
    }, () =>
    {
      this.isSaving = false;
    });
  }
}
