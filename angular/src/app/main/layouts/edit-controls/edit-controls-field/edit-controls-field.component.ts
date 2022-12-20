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
  fieldValue : string | undefined;

  optionsVisible : boolean = false;
  optionsModel : MenuItem[] = [];

  freeTextVisible : boolean = false;
  freeTextText : string | undefined;

  constructor(private apiService: ApiService)
  {
    //
  }

  ngOnInit(): void {
    this.fieldLabel = this.field!.property.Name;
    this.fieldValue = this.field!.property.UpdatedValue;
    if (this.fieldValue === undefined || this.fieldValue === null)
    {
      this.fieldValue = this.field!.property.DefaultValue;
    }

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

    this.freeTextVisible = this.field!.control.AllowTextEdit;
    this.freeTextText = this.fieldValue;
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
      this.fieldValue = stringValue;
      this.freeTextText = stringValue;
      this.isSaving = false;
    }, () =>
    {
      this.isSaving = false;
    });
  }
}
