import { Component } from '@angular/core';

import {MenuItem} from 'primeng/api';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent {

  navItems: MenuItem[] = [];
  navActiveItem: MenuItem | undefined;

  constructor() {
    // Create nav items
    this.navItems = [
      { label: 'Profile', icon: 'pi pi-fw pi-home', routerLink: "/admin/viewCurrentUser" }
    ];
    this.navActiveItem = this.navItems[0];
  }
}
