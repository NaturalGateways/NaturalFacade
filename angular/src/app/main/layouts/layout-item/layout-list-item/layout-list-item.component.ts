import { Component, Input } from '@angular/core';

import { environment } from '../../../../../environments/environment';

import { ContextMenu } from 'primeng/contextmenu';

import { LayoutItem } from '../../layout-item/layout-item-data';

@Component({
  selector: 'app-layout-list-item',
  templateUrl: './layout-list-item.component.html',
  styleUrls: ['./layout-list-item.component.css']
})
export class LayoutListItemComponent {

  @Input() item: LayoutItem | undefined;

  onViewOverlay()
  {
    var newUrl : string = window.location.protocol + "//" + window.location.host + "/overlay?layoutId=" + this.item?.LayoutId;
    if (environment.openLinkNewTab)
    {
      window.open(newUrl, "_blank");
    }
    else
    {
      location.href = newUrl;
    }
  }

  onViewControlsForLayout(event: MouseEvent, contextMenu : ContextMenu)
  {
    // Go straight to item
    if (this.item!.ControlsMenuItems.length === 1)
    {
      this.item?.ControlsMenuItems[0]?.command?.call(null);
      return;
    }

    // Show menu
    contextMenu.show(event);
    event.stopPropagation();
  }
}
