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

  getIdToken(successCallback: (idToken: string) => void, errorCallback: () => void)
  {
    // Check if refresh is needed
    var apiAuthModel = this.apiAuthModel!;
    var curDate : Date = new Date();
    if (curDate.toISOString() < apiAuthModel.access.expiryDateTime)
    {
      if (environment.production === false)
      {
        console.log("Token exists.");
      }
      successCallback(apiAuthModel.access.idToken);
      return;
    }

    // Do refresh
    this.refreshToken(successCallback, errorCallback);
  }

  getRegisterUrl(cognitoClientId: string, callbackUrl: string) : string
  {
    return environment.cognitoUrl + "/signup?client_id=" + cognitoClientId + "&response_type=code&scope=openid&redirect_uri=" + callbackUrl;
  }

  getLoginUrl(cognitoClientId: string, callbackUrl: string) : string
  {
    return environment.cognitoUrl + "/login?response_type=code&scope=openid&client_id=" + cognitoClientId + "&redirect_uri=" + callbackUrl;
  }

  authenticate(apiService: ApiService, code: string, callback: () => void)
  {
    // Set UI
    this.authentication = CognitoServiceAuthStatus.LoggingIn;
    this.authEmitter.emit();

    // Use Cognito to convert code to tokens
    this.settingsService.getCognitoClientIdAndCallbackUrl((cognitoClientId, callbackUrl) =>
    {
      let url: string = environment.cognitoUrl + "/oauth2/token?grant_type=authorization_code&client_id=" + cognitoClientId + "&code=" + code + "&redirect_uri=" + callbackUrl;
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

  refreshToken(successCallback: (idToken: string) => void, errorCallback: () => void)
  {
    // Use Cognito to convert code to tokens
    this.settingsService.getCognitoClientId((cognitoClientId) =>
    {
      var apiAuthModel = this.apiAuthModel!;
      var refreshToken = apiAuthModel.access.refreshToken;
      let url: string = environment.cognitoUrl + "/oauth2/token?grant_type=refresh_token&client_id=" + cognitoClientId + "&refresh_token=" + this.apiAuthModel?.access.refreshToken;
      const headers = new HttpHeaders().set("Content-Type", "application/x-www-form-urlencoded");
      this.http.post<CognitoServiceTokenResponseDto>(url, "", {headers}).subscribe(resp => {
        let cognitoAccess = new CognitoAccessModel(resp.id_token!, refreshToken, resp.access_token!, resp.expires_in!);
        this.apiAuthModel = new ApiAuthModel(apiAuthModel.user, cognitoAccess);
        localStorage.setItem('auth', JSON.stringify(this.apiAuthModel));
        this.authentication = CognitoServiceAuthStatus.Authenticated;
        this.authEmitter.emit();
        successCallback(resp.id_token!);
        if (environment.production === false)
        {
          console.log("Refreshed token.");
        }
      }, error => {
        console.log("Error: " + JSON.stringify(error));
        this.authentication = CognitoServiceAuthStatus.None;
        this.authEmitter.emit();
        errorCallback();
      });
    });
  }
}
