import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { MenuItem } from 'primeng/api';

import { ApiService } from '../../api/api.service';

import { LayoutItem } from './layout-item/layout-item-data';

@Component({
  selector: 'app-layouts',
  templateUrl: './layouts.component.html',
  styleUrls: ['./layouts.component.css']
})
export class LayoutsComponent {

  layouts: LayoutItem[] = [];

  constructor(private router: Router, private apiService: ApiService) {
    this.apiService.getLayoutPage((layouts) =>
    {
      var layoutItems: LayoutItem[] = [];
      layouts.forEach((element) => {
        var layoutItem = new LayoutItem();
        layoutItem.LayoutId = element.LayoutId;
        layoutItem.Name = element.Name;
        var dateTime = new Date(element.CreatedDateTime);
        layoutItem.CreatedDate = dateTime.toLocaleDateString();
        layoutItem.CreatedTime = dateTime.toLocaleTimeString();
        if (element.ControlsNameArray !== null && element.ControlsNameArray !== undefined)
        {
          element.ControlsNameArray.forEach((controlsName) =>
          {
            var controlsIndex : number = layoutItem.ControlsMenuItems.length;
            var controlsMenuItems : MenuItem =
            {
              label: controlsName,
              icon:'pi pi-fw pi-sliders-v',
              command: e => this.onViewControlsAtIndex(layoutItem.LayoutId, controlsIndex)
            };
            layoutItem.ControlsMenuItems.push(controlsMenuItems);
          });
        }
        layoutItems.push(layoutItem);
      });
      this.layouts = layoutItems;
    }, () =>
    {
      //
    });
  }

  onViewControlsAtIndex(layoutId: string, controlsIndex: number)
  {
    var newUrl : string = window.location.protocol + "//" + window.location.host + "/layouts/controls/edit?layoutId=" + layoutId + "&controlsIndex=" + controlsIndex;
    window.open(newUrl, "_blank");
  }
}
