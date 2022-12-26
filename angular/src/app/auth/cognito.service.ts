import { Injectable, EventEmitter  } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

import { SettingsService } from '../services/settings.service';

import { environment } from '../../environments/environment';

import { ApiService } from '../api/api.service';

import { ApiAuthModel } from '../api/api-auth-model';

import { CognitoAccessModel } from './cognito-model';

class CognitoServiceTokenResponseDto
{
  id_token: string | undefined;
  refresh_token: string | undefined;
  access_token: string | undefined;
  expires_in: number = 0;
}
export enum CognitoServiceAuthStatus
{
  None,
  LoggingIn,
  Authenticated
}

@Injectable({
  providedIn: 'root'
})
export class CognitoService {
  authEmitter: EventEmitter<any> = new EventEmitter();

  apiAuthModel: ApiAuthModel | null = null;
  authentication: CognitoServiceAuthStatus = CognitoServiceAuthStatus.None;

  constructor(private settingsService: SettingsService, private http: HttpClient, private router: Router)
  {
    // Check auth from storage
    var authJson: string | null = localStorage.getItem('auth');
    if (authJson !== null)
    {
      this.apiAuthModel = JSON.parse(authJson);
      this.authentication = CognitoServiceAuthStatus.Authenticated;
      if (environment.production)
      {
        this.router.navigate(['/dashboard']);
      }
    }
  }

  getRegisterUrl(callbackUrl: string) : string
  {
    return environment.cognitoUrl + "/signup?client_id=" + environment.cognitoClientId + "&response_type=code&scope=openid&redirect_uri=" + callbackUrl;
  }

  getLoginUrl(callbackUrl: string) : string
  {
    return environment.cognitoUrl + "/login?response_type=code&scope=openid&client_id=" + environment.cognitoClientId + "&redirect_uri=" + callbackUrl;
  }

  authenticate(apiService: ApiService, code: string, callback: () => void)
  {
    // Set UI
    this.authentication = CognitoServiceAuthStatus.LoggingIn;
    this.authEmitter.emit();

    // Use Cognito to convert code to tokens
    this.settingsService.getCognitoCallbackUrl((callbackUrl) =>
    {
      let url: string = environment.cognitoUrl + "/oauth2/token?grant_type=authorization_code&client_id=" + environment.cognitoClientId + "&code=" + code + "&redirect_uri=" + callbackUrl;
      const headers = new HttpHeaders().set("Content-Type", "application/x-www-form-urlencoded");
      this.http.post<CognitoServiceTokenResponseDto>(url, "", {headers}).subscribe(resp => {
        let cognitoAccess = new CognitoAccessModel(resp.id_token!, resp.refresh_token!, resp.access_token!, resp.expires_in!);
        apiService.getCurrentUser(cognitoAccess, (userResponse) =>
        {
          this.apiAuthModel = new ApiAuthModel(userResponse, cognitoAccess);
          localStorage.setItem('auth', JSON.stringify(this.apiAuthModel));
          this.authentication = CognitoServiceAuthStatus.Authenticated;
          this.authEmitter.emit();
          callback();
        }, () =>
        {
          this.authentication = CognitoServiceAuthStatus.None;
          this.authEmitter.emit();
          callback();
        });
      }, error => {
        console.log("Error: " + JSON.stringify(error));
        this.authentication = CognitoServiceAuthStatus.None;
        this.authEmitter.emit();
        callback();
      });
    });
  }

  logout()
  {
    localStorage.removeItem('auth');
    this.apiAuthModel = null;
    this.authentication = CognitoServiceAuthStatus.None;
    this.authEmitter.emit();
  }
}
