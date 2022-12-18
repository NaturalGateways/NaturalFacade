import { MenuItem } from 'primeng/api';

export class LayoutItem {
  LayoutId: string = "";
  Name: string = "";
  CreatedDate: string = "";
  CreatedTime: string = "";
  ControlsMenuItems: MenuItem[] = [];
}

export class LayoutItemControls {
  ControlsIndex: number = 0;
  ControlsName: string = "";
}
