import { Component } from '@angular/core';

import { ApiService } from '../../api/api.service';

import { LayoutSummaryApiDto } from '../../api/api-auth-dto/layout-summary-api-dto';

export class LayoutItem {
  LayoutId: string = "";
  Name: string = "";
  CreatedDate: string = "";
  CreatedTime: string = "";
}

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
        layoutItems.push(layoutItem);
      });
      this.layouts = layoutItems;
    }, () =>
    {
      //
    });
  }

  onViewOverlay(layoutId: string)
  {
    var newUrl : string = window.location.protocol + "//" + window.location.host + "/overlay?layoutId=" + layoutId;
    window.open(newUrl, "_blank");
  }
}
