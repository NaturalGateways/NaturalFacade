import { Component } from '@angular/core';

import { MenuItem } from 'primeng/api';
import { ContextMenu } from 'primeng/contextmenu';

import { ApiService } from '../../api/api.service';

import { LayoutItem, LayoutItemControls } from './layout-item/layout-item-data';

@Component({
  selector: 'app-layouts',
  templateUrl: './layouts.component.html',
  styleUrls: ['./layouts.component.css']
})
export class LayoutsComponent {

  layouts: LayoutItem[] = [];

  constructor(private apiService: ApiService) {
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
            var controlsMenuItems : MenuItem =
            {
              label: controlsName,
              icon:'pi pi-fw pi-sliders-v',
              command: e => this.onViewControlsAtIndex(layoutItem.LayoutId, layoutItem.ControlsMenuItems.length)
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
    console.log("Layout: " + layoutId + ", " + controlsIndex);
  }
}
