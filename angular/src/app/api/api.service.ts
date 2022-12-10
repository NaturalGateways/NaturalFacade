import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { environment } from '../../environments/environment';

import { CognitoService } from '../auth/cognito.service';
import { CognitoAccessModel } from '../auth/cognito-model';

import { LayoutApiDto } from '../layout/model/layout-api-dto';

import { BaseResponseDto, BlankResponseDto } from './base-dto';
import { CurrentUserApiDto } from './api-auth-dto/current-user-api-dto';
import { LayoutSummaryApiDto } from './api-auth-dto/layout-summary-api-dto';

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

  getLayoutPage(successCallback: (layouts: LayoutSummaryApiDto[]) => void, errorCallback: () => void)
  {
    let url: string = environment.apiUrl + "/auth";
    const headers = new HttpHeaders().set("Content-Type", "application/json").set("Authorization", this.cognitoService.apiAuthModel?.access.idToken!);
    var reqBody = {RequestType: "GetLayoutSummaryPage"};
    this.http.post<BaseResponseDto<LayoutSummaryApiDto[]>>(url, reqBody, {headers}).subscribe(resp => {
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

  convertLayout(layoutObj: any, successCallback: (overlayObj: LayoutApiDto) => void, errorCallback: () => void)
  {
    let url: string = environment.apiUrl + "/anon";
    const headers = new HttpHeaders().set("Content-Type", "application/json");
    var reqBody = {RequestType: "ConvertLayoutToOverlay", LayoutConfig: layoutObj};
    this.http.post<BaseResponseDto<LayoutApiDto>>(url, reqBody, {headers}).subscribe(resp => {
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

  getLayout(layoutId: string, successCallback: (layoutObj: any) => void, errorCallback: () => void)
  {
    let url: string = environment.apiUrl + "/auth";
    const headers = new HttpHeaders().set("Content-Type", "application/json").set("Authorization", this.cognitoService.apiAuthModel?.access.idToken!);
    var reqBody = {RequestType: "GetLayout", GetLayout: {LayoutId:layoutId}};
    this.http.post<BaseResponseDto<any>>(url, reqBody, {headers}).subscribe(resp => {
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

  createLayout(layoutName: string, successCallback: () => void, errorCallback: () => void)
  {
    let url: string = environment.apiUrl + "/auth";
    const headers = new HttpHeaders().set("Content-Type", "application/json").set("Authorization", this.cognitoService.apiAuthModel?.access.idToken!);
    var reqBody = {RequestType: "CreateLayout", CreateLayout: {Name:layoutName}};
    this.http.post<BlankResponseDto>(url, reqBody, {headers}).subscribe(resp => {
      if (resp.Success)
      {
        successCallback();
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

  putLayout(layoutId: string, layoutConfigString: string, successCallback: () => void, errorCallback: () => void)
  {
    let url: string = environment.apiUrl + "/auth";
    const headers = new HttpHeaders().set("Content-Type", "application/json").set("Authorization", this.cognitoService.apiAuthModel?.access.idToken!);
    var reqBody = {RequestType: "PutLayout", PutLayout: {LayoutId:layoutId,LayoutConfig:JSON.parse(layoutConfigString)}};
    this.http.post<BlankResponseDto>(url, reqBody, {headers}).subscribe(resp => {
      if (resp.Success)
      {
        successCallback();
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
