import { JsonPipe } from '@angular/common';
import { Component, Input } from '@angular/core';

import { MenuItem } from 'primeng/api';
import { ContextMenu } from 'primeng/contextmenu';
import { InputSwitchOnChangeEvent } from 'primeng/inputswitch';

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
  savedTimerRunning : boolean = false;
  savedTimerSecs : number | undefined;
  savedTimerStartDateTime : Date | undefined;
  savedAudioState : string | undefined;

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

  timerVisible : boolean = false;
  timerRunning : boolean = false;
  timerText : string | undefined;

  audioWalkmanVisible : boolean = false;
  isAudioWalkmanStopped() { return this.savedAudioState === 'Stopped'; }
  isAudioWalkmanPaused() { return this.savedAudioState === 'Paused'; }
  isAudioWalkmanPlaying() { return this.savedAudioState === 'Playing'; }

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

    // Setup timer
    if (this.field!.fieldDef.Timer !== undefined)
    {
      this.timerVisible = true;
    }

    // Setup audio walkman
    if (this.field!.fieldDef.AudioWalkman !== undefined)
    {
      this.audioWalkmanVisible = true;
    }

    // Set value
    this.setSavedValue(this.field!.value);
  }

  setSavedValue(value: any)
  {
    switch (this.field!.valueType)
    {
      case "Audio":
        this.savedAudioState = String(value.State);
        break;
      case "Boolean":
        this.savedBoolValue = Boolean(value);
        this.switchValue = this.savedBoolValue;
        this.savedStringValue = this.savedBoolValue ? this.switchTrueLabel : this.switchFalseLabel;
        break;
      case "String":
        this.savedStringValue = value;
        this.textFieldText = value;
        if (this.field!.fieldDef.Integer !== undefined)
        {
          this.savedNumberValue = Number(value);
          this.integerValue = this.savedNumberValue;
        }
        break;
        case "Timer":
          this.savedTimerRunning = value.StartDateTime !== undefined;
          this.savedTimerSecs = value.Secs;
          this.timerRunning = this.savedTimerRunning;
          if (this.savedTimerRunning)
          {
            this.savedTimerStartDateTime = new Date(value.StartDateTime);
            this.timerText = "Running";
          }
          else
          {
            this.savedTimerStartDateTime = undefined;
            var date = new Date(2000, 1, 1, 0, 0, this.savedTimerSecs, 0);
            this.timerText = date.toLocaleTimeString();
          }
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

  saveTimerRunning(event: InputSwitchOnChangeEvent)
  {
    if (event.checked)
    {
      var curDateTime = new Date();
      var curDateTimeUtc = curDateTime.toISOString();
      this.upload({Secs:this.savedTimerSecs,StartDateTime:curDateTimeUtc});
    }
    else
    {
      // Work out new timespan in secs
      var curDateTime = new Date();
      var deltaMillis = curDateTime.getTime() - this.savedTimerStartDateTime!.getTime();
      var deltaSecs = Math.floor(deltaMillis / 1000);
      var newTimerSecs = this.savedTimerSecs! + this.field!.fieldDef.Timer.Direction * deltaSecs;
      // Clamp to limits
      if (this.field!.fieldDef.Timer.MinValue !== undefined && newTimerSecs < this.field!.fieldDef.Timer.MinValue)
        newTimerSecs = this.field!.fieldDef.Timer.MinValue;
      if (this.field!.fieldDef.Timer.MaxValue !== undefined && this.field!.fieldDef.Timer.MaxValue < newTimerSecs)
        newTimerSecs = this.field!.fieldDef.Timer.MaxValue;
      // Upload
      this.upload({Secs:newTimerSecs});
    }
  }

  clearTimer()
  {
    this.upload(this.field!.defaultValue);
  }

  onAudioWalkmanStop()
  {
    this.upload({'State':'Stopped'});
  }

  onAudioWalkmanPause()
  {
    switch (this.savedAudioState)
    {
      case 'Stopped':
        break;
      case 'Paused':
        this.upload({'State':'Playing'});
        break;
      case 'Playing':
        this.upload({'State':'Paused'});
        break;
    }
  }

  onAudioWalkmanPlay()
  {
    this.upload({'State':'Playing'});
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
