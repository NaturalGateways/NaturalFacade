import { Injectable, EventEmitter  } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { environment } from '../../environments/environment';

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

class CognitoServiceAuth
{
  refreshToken: string;
  accessToken: string;
  expiryDateTime: Date;

  constructor(responseDto: CognitoServiceTokenResponseDto)
  {
    this.refreshToken = responseDto.refresh_token!;
    this.accessToken = responseDto.access_token!;
    this.expiryDateTime = new Date();
    this.expiryDateTime.setSeconds(this.expiryDateTime.getSeconds() + responseDto.expires_in);
    this.expiryDateTime.setMinutes(this.expiryDateTime.getMinutes() - 5);
    if (environment.production === false)
    {
      console.log("ID: " + responseDto.id_token!);
      console.log("Access: " + responseDto.access_token!);
      console.log("Refresh: " + responseDto.refresh_token!);
    }
  }
}

@Injectable({
  providedIn: 'root'
})
export class CognitoService {
  authEmitter: EventEmitter<any> = new EventEmitter();

  registerUrl: string;
  signInUrl: string;

  authentication: CognitoServiceAuthStatus = CognitoServiceAuthStatus.None;

  auth: CognitoServiceAuth | undefined;

  constructor(private http: HttpClient)
  {
    this.registerUrl = environment.cognitoUrl + "/signup?client_id=" + environment.cognitoClientId + "&response_type=code&scope=openid&redirect_uri=" + environment.callbackUrl;
    this.signInUrl = environment.cognitoUrl + "/login?response_type=code&scope=openid&client_id=" + environment.cognitoClientId + "&redirect_uri=" + environment.callbackUrl;
  }

  authenticate(code: string, callback: () => void)
  {
    this.authentication = CognitoServiceAuthStatus.LoggingIn;
    this.authEmitter.emit();
    let url: string = environment.cognitoUrl + "/oauth2/token?grant_type=authorization_code&client_id=" + environment.cognitoClientId + "&code=" + code + "&redirect_uri=" + environment.callbackUrl;
    const headers = new HttpHeaders().set("Content-Type", "application/x-www-form-urlencoded");
    this.http.post<CognitoServiceTokenResponseDto>(url, "", {headers}).subscribe(resp => {
      this.auth = new CognitoServiceAuth(resp);
      this.authentication = CognitoServiceAuthStatus.Authenticated;
      this.authEmitter.emit();
      callback();
    }, error => {
      console.log("Error: " + JSON.stringify(error));
      this.authentication = CognitoServiceAuthStatus.None;
      this.authEmitter.emit();
      callback();
    });
  }

  logout()
  {
    this.auth = undefined;
    this.authentication = CognitoServiceAuthStatus.None;
    this.authEmitter.emit();
  }
}
