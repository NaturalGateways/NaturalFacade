import { Component, Input } from '@angular/core';

import { MenuItem } from 'primeng/api';
import { ContextMenu } from 'primeng/contextmenu';

import { EditControlsField } from '../edit-controls-model';

@Component({
  selector: 'app-edit-controls-field',
  templateUrl: './edit-controls-field.component.html',
  styleUrls: ['./edit-controls-field.component.css']
})
export class EditControlsFieldComponent {
  @Input() field : EditControlsField | undefined;

  fieldLabel : string | undefined;
  fieldValue : string | undefined;

  optionsVisible : boolean = false;
  optionsModel : MenuItem[] = [];

  freeTextVisible : boolean = false;
  freeTextText : string | undefined;

  ngOnInit(): void {
    this.fieldLabel = this.field!.property.Name;
    this.fieldValue = this.field!.property.DefaultValue;

    if (this.field!.control.Options !== undefined && this.field!.control.Options !== null)
    {
      this.optionsVisible = true;
      var optionValues : string[] = this.field!.control.Options;
      optionValues.forEach(optionValue => {
        var controlsMenuItems : MenuItem =
        {
          label: optionValue,
          icon:'pi pi-fw pi-stop'
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
    //
  }
}
