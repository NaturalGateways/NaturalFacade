import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

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

  constructor(private router: Router) { }

  onViewOverlay()
  {
    if (environment.openLinkNewTab)
    {
      var newUrl : string = window.location.protocol + "//" + window.location.host + "/overlay?layoutId=" + this.item?.LayoutId;
      window.open(newUrl, "_blank");
    }
    else
    {
      this.router.navigate(['/overlay'], { queryParams: { layoutId: this.item!.LayoutId } });
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
