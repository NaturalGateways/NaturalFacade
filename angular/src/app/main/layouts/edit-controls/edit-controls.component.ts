import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ApiService } from '../../../api/api.service';

import { EditControlsField } from './edit-controls-model';

@Component({
  selector: 'app-edit-controls',
  templateUrl: './edit-controls.component.html',
  styleUrls: ['./edit-controls.component.css']
})
export class EditControlsComponent {
  layoutId: string | null = null;
  controlsIndex: number | null = null;

  isLoaded: boolean = false;

  fields: EditControlsField[] = [];

  constructor(private route: ActivatedRoute, private apiService: ApiService)
  {
    //
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.layoutId = params['layoutId'];
      this.controlsIndex = Number(params['controlsIndex']);
      this.apiService.getLayoutControls(this.layoutId!, this.controlsIndex!, (response) =>
      {
        response.controls!.Fields?.forEach((child: any) =>
        {
          let propertyIndex : number = child.PropIndex as number;
          let field = new EditControlsField();
          field.control = child;
          field.property = response.properties![propertyIndex];
          this.fields.push(field);
        });
        this.isLoaded = true;
      }, () =>
      {
        //
      });
    });
  }

  onSave()
  {
    //
  }
}
