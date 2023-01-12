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
  fieldStringValue : string | null = null;
  fieldBoolValue : boolean | null = null;

  switchVisible : boolean = false;
  switchFalseLabel : string = "False";
  switchTrueLabel : string = "True";
  switchSavedValue : boolean = false;
  switchEditedValue : boolean = false;

  numericVisible : boolean = false;
  numericValue : number = 0;
  minValue : number = 0;
  maxValue : number = 0;

  optionsVisible : boolean = false;
  optionsModel : MenuItem[] = [];

  freeTextVisible : boolean = false;
  freeTextText : string | undefined;

  constructor(private apiService: ApiService)
  {
    //
  }

  ngOnInit(): void {
    this.fieldLabel = this.field!.control.Label;
    var fieldValue : any = this.field!.property.UpdatedValue ?? this.field!.property.DefaultValue;

    // Check type
    switch (this.field!.property.ValueType)
    {
      // String
      case 0:
        this.fieldStringValue = fieldValue;
        this.freeTextText = fieldValue;
        break;
      // Boolean
      case 1:
        this.fieldBoolValue = Boolean(fieldValue);
        this.freeTextText = this.fieldBoolValue ? this.switchTrueLabel : this.switchFalseLabel;
        break;
    }

    // Setup switch
    if (this.field!.control.Switch !== undefined && this.field!.control.Switch !== null)
    {
      this.switchVisible = true;
      var switchObj = this.field!.control.Switch;
      if (switchObj.FalseLabel !== undefined && switchObj.FalseLabel !== null)
        this.switchFalseLabel = switchObj.FalseLabel;
      if (switchObj.TrueLabel !== undefined && switchObj.TrueLabel !== null)
        this.switchTrueLabel = switchObj.TrueLabel;
      this.switchSavedValue = this.fieldBoolValue ?? false;
      this.switchEditedValue = this.switchSavedValue;
    }

    // Setup numeric
    if (this.field!.control.Integer !== undefined && this.field!.control.Integer !== null)
    {
      this.numericVisible = true;
      this.numericValue = Number(fieldValue);
      if (this.field!.control.Integer.MinValue !== undefined && this.field!.control.Integer.MinValue !== null)
        this.minValue = this.field!.control.Integer.MinValue;
      else
        this.minValue = Number.MIN_VALUE;
      if (this.field!.control.Integer.MaxValue !== undefined && this.field!.control.Integer.MaXValue !== null)
        this.maxValue = this.field!.control.Integer.MaxValue;
      else
        this.maxValue = Number.MAX_VALUE;
    }

    // Setup choices
    if (this.field!.control.Options !== undefined && this.field!.control.Options !== null)
    {
      this.optionsVisible = true;
      var optionValues : string[] = this.field!.control.Options;
      optionValues.forEach(optionValue => {
        var controlsMenuItems : MenuItem =
        {
          label: optionValue,
          icon:'pi pi-fw pi-stop',
          command: e => this.saveString(optionValue)
        };
        this.optionsModel.push(controlsMenuItems);
      });
    }

    // Setup free text
    this.freeTextVisible = this.field!.control.AllowTextEdit;
  }

  onSwitchToggled(event: any)
  {
    this.saveBoolean(event.checked);
  }

  onNumericChanged() {
    this.saveString(String(this.numericValue));
  }

  onNumericClear() {
    this.saveString(this.field!.property.DefaultValue);
  }

  onShowContextMenu(event: MouseEvent, contextMenu : ContextMenu)
  {
    // Show menu
    contextMenu.show(event);
    event.stopPropagation();
  }

  saveFreeText()
  {
    this.saveString(this.freeTextText!);
  }

  saveString(stringValue: string)
  {
    this.isSaving = true;
    var propIndex: number = this.field!.control.PropIndex;
    this.apiService.putLayoutPropertyValue(this.layoutId!, propIndex, stringValue, () =>
    {
      this.fieldStringValue = stringValue;
      this.fieldBoolValue = null;
      this.numericValue = Number(stringValue);
      this.freeTextText = stringValue;
      this.isSaving = false;
    }, () =>
    {
      this.isSaving = false;
    });
  }

  saveBoolean(boolValue: boolean)
  {
    this.isSaving = true;
    var propIndex: number = this.field!.control.PropIndex;
    this.apiService.putLayoutPropertyValue(this.layoutId!, propIndex, boolValue, () =>
    {
      var stringValue : string = boolValue ? this.switchTrueLabel : this.switchFalseLabel;
      this.fieldStringValue = null;
      this.fieldBoolValue = boolValue;
      this.numericValue = Number(stringValue);
      this.freeTextText = stringValue;
      this.switchSavedValue = boolValue;
      this.isSaving = false;
    }, () =>
    {
      this.isSaving = false;
    });
  }
}
