import { Component } from '@angular/core';

import { SettingsService } from '../services/settings.service';

import { ContextMenu } from 'primeng/contextmenu';
import { MenuItem } from 'primeng/api';

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

  constructor(private settingsService: SettingsService, public cognitoService: CognitoService)
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
    this.settingsService.getCognitoClientIdAndCallbackUrl((cognitoClientId, callbackUrl) =>
    {
      location.href = this.cognitoService.getRegisterUrl(cognitoClientId, callbackUrl);
    });
  }

  onLoginClicked()
  {
    this.settingsService.getCognitoClientIdAndCallbackUrl((cognitoClientId, callbackUrl) =>
    {
      location.href = this.cognitoService.getLoginUrl(cognitoClientId, callbackUrl);
    });
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
