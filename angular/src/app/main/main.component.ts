import { Component, OnInit } from '@angular/core';

import {MenuItem} from 'primeng/api';

import { CognitoService, CognitoServiceAuthStatus } from '../auth/cognito.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit {

  isAuthenticated: boolean = false;

  navItems: MenuItem[] = [];
  navActiveItem: MenuItem | undefined;

  constructor(public cognitoService: CognitoService) {
    // Create nav items
    this.navItems = [
      { label: 'Home', icon: 'pi pi-fw pi-home', routerLink: "/dashboard" },
      { label: 'Layouts', icon: 'pi pi-fw pi-file', routerLink: "/layouts" }
    ];
    this.navActiveItem = this.navItems[0];
  }

  ngOnInit(): void {
    // Initialise from service
    this.isAuthenticated = this.cognitoService.authentication === CognitoServiceAuthStatus.Authenticated;

    // Subscribe to auth service
    this.cognitoService.authEmitter.subscribe(() =>
    {
      setTimeout(() =>
      {
        this.isAuthenticated = this.cognitoService.authentication === CognitoServiceAuthStatus.Authenticated;
      });
    });
  }
}
