import { Component } from '@angular/core';

import { CognitoService, CognitoServiceAuthStatus } from './auth/cognito.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Natural Facade';

  isAuthOut: boolean = true;
  isAuthIn: boolean = false;

  constructor(public cognitoService: CognitoService) { }

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

  onLogoutClicked()
  {
    this.cognitoService.logout();
    location.href = "/";
  }
}
