import { Component } from '@angular/core';

import {ContextMenu} from 'primeng/contextmenu';
import {MenuItem} from 'primeng/api';

import { ApiService } from './../api/api.service';
import { CognitoService, CognitoServiceAuthStatus } from './../auth/cognito.service';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.css']
})
export class RootComponent {

  isAuthOut: boolean = true;
  isAuthIn: boolean = false;

  contextMenuItems: MenuItem[] = [];

  constructor(public cognitoService: CognitoService, public apiService: ApiService)
  {
    this.contextMenuItems = [
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        routerLink: '/admin/viewCurrentUser'
      },
      {
         separator:true
      },
      {
        label: 'Logout',
        icon: 'pi pi-fw pi-sign-out',
        command: (event) => { this.onLogoutClicked(); }
      }
    ];
  }

  ngOnInit()
  {
    // Initialise from service
    this.isAuthOut = this.cognitoService.authentication === CognitoServiceAuthStatus.None;
    this.isAuthIn = this.cognitoService.authentication === CognitoServiceAuthStatus.Authenticated;

    // Subscribe to auth service
    this.cognitoService.authEmitter.subscribe(() =>
    {
      setTimeout(() =>
      {
        this.isAuthOut = this.cognitoService.authentication === CognitoServiceAuthStatus.None;
        this.isAuthIn = this.cognitoService.authentication === CognitoServiceAuthStatus.Authenticated;
      });
    });
  }

  onRegisterClicked()
  {
    location.href = this.cognitoService.registerUrl;
  }

  onLoginClicked()
  {
    location.href = this.cognitoService.signInUrl;
  }

  onUserClicked(event: MouseEvent, contextMenu: ContextMenu): void {
    event.stopPropagation();
    event.preventDefault();
    contextMenu.show(event);
  }

  onLogoutClicked()
  {
    this.cognitoService.logout();
    location.href = "/";
  }
}
