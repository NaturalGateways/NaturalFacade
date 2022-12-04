import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { environment } from '../../environments/environment';

import { CognitoService } from '../auth/cognito.service';
import { CognitoAccessModel } from '../auth/cognito-model';

import { BaseResponseDto } from './base-dto';
import { ApiAuthModel } from './api-auth-model';
import { CurrentUserApiDto } from './api-auth-dto/current-user-api-dto';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(public cognitoService: CognitoService, private http: HttpClient) { }

  getCurrentUser(cognitoAccessModel: CognitoAccessModel, successCallback: (response: CurrentUserApiDto) => void, errorCallback: () => void)
  {
    let url: string = environment.apiUrl + "/auth";
    const headers = new HttpHeaders().set("Content-Type", "application/json").set("Authorization", cognitoAccessModel.idToken);
    var reqBody = {RequestType: "GetCurrentUser"};
    this.http.post<BaseResponseDto<CurrentUserApiDto>>(url, reqBody, {headers}).subscribe(resp => {
      if (resp.Success && resp.Payload !== undefined)
      {
        successCallback(resp.Payload);
      }
      else
      {
        console.log("Error: " + JSON.stringify(resp));
        errorCallback();
      }
    }, error => {
      console.log("Error: " + JSON.stringify(error));
      errorCallback();
    });
  }

  updateCurrentUser(newName: string, successCallback: (response: CurrentUserApiDto) => void, errorCallback: () => void)
  {
    let url: string = environment.apiUrl + "/auth";
    const headers = new HttpHeaders().set("Content-Type", "application/json").set("Authorization", this.cognitoService.apiAuthModel?.access.idToken!);
    var reqBody = {RequestType: "UpdateCurrentUser", UpdateCurrentUser: {Name:newName}};
    this.http.post<BaseResponseDto<CurrentUserApiDto>>(url, reqBody, {headers}).subscribe(resp => {
      if (resp.Success && resp.Payload !== undefined)
      {
        successCallback(resp.Payload);
      }
      else
      {
        console.log("Error: " + JSON.stringify(resp));
        errorCallback();
      }
    }, error => {
      console.log("Error: " + JSON.stringify(error));
      errorCallback();
    });
  }
}
