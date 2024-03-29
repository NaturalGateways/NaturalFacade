import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { SettingsService } from '../services/settings.service';

import { CognitoService } from '../auth/cognito.service';
import { CognitoAccessModel } from '../auth/cognito-model';

import { ConvertedOverlayApiDto, OverlayApiDto } from '../layout/model/layout-api-dto';

import { BaseResponseDto, BlankResponseDto } from './base-dto';

import { CurrentUserApiDto } from './api-auth-dto/current-user-api-dto';
import { LayoutSummaryApiDto } from './api-auth-dto/layout-summary-api-dto';

import { LayoutControlsApiDto } from './api-dto/layout-controls-api-dto';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private settingsService: SettingsService, public cognitoService: CognitoService, private http: HttpClient) { }

  executeAnonGetWithResponse<ResponseDto>(queryParams: string, successCallback: (response: ResponseDto) => void, errorCallback: () => void)
  {
    this.settingsService.getApiUrl((apiUrl) =>
    {
      let url: string = apiUrl + "/anon?" + queryParams;
      const headers = new HttpHeaders().set("Content-Type", "application/json");
      this.http.get<BaseResponseDto<ResponseDto>>(url, {headers}).subscribe(resp => {
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
    });
  }

  executeAnonPostWithResponse<ResponseDto>(reqBody: any, successCallback: (response: ResponseDto) => void, errorCallback: () => void)
  {
    this.settingsService.getApiUrl((apiUrl) =>
    {
      let url: string = apiUrl + "/anon";
      const headers = new HttpHeaders().set("Content-Type", "application/json");
      this.http.post<BaseResponseDto<ResponseDto>>(url, reqBody, {headers}).subscribe(resp => {
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
    });
  }

  executeAuthPostNoResponse(reqBody: any, successCallback: () => void, errorCallback: () => void)
  {
    this.settingsService.getApiUrl((apiUrl) =>
    {
      this.cognitoService.getIdToken((idToken) =>
      {
        let url: string = apiUrl + "/auth";
        const headers = new HttpHeaders().set("Content-Type", "application/json").set("Authorization", idToken);
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
      }, errorCallback);
    });
  }

  executeAuthPostWithResponse<ResponseDto>(reqBody: any, successCallback: (response: ResponseDto) => void, errorCallback: () => void)
  {
    this.settingsService.getApiUrl((apiUrl) =>
    {
      this.cognitoService.getIdToken((idToken) =>
      {
        let url: string = apiUrl + "/auth";
        const headers = new HttpHeaders().set("Content-Type", "application/json").set("Authorization", idToken);
        this.http.post<BaseResponseDto<ResponseDto>>(url, reqBody, {headers}).subscribe(resp => {
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
      }, errorCallback);
    });
  }

  getCurrentUser(cognitoAccessModel: CognitoAccessModel, successCallback: (response: CurrentUserApiDto) => void, errorCallback: () => void)
  {
    this.settingsService.getApiUrl((apiUrl) =>
    {
      let url: string = apiUrl + "/auth";
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
    });
  }

  getInfo(successCallback: (response: any) => void, errorCallback: () => void)
  {
    let queryParams: string = "RequestType=GetInfo";
    this.executeAnonGetWithResponse<any>(queryParams, successCallback, errorCallback);
  }

  updateCurrentUser(newName: string, successCallback: (response: CurrentUserApiDto) => void, errorCallback: () => void)
  {
    var reqBody = {RequestType: "UpdateCurrentUser", UpdateCurrentUser: {Name:newName}};
    this.executeAuthPostWithResponse<CurrentUserApiDto>(reqBody, successCallback, errorCallback);
  }

  getLayoutPage(successCallback: (layouts: LayoutSummaryApiDto[]) => void, errorCallback: () => void)
  {
    var reqBody = {RequestType: "GetLayoutSummaryPage"};
    this.executeAuthPostWithResponse<LayoutSummaryApiDto[]>(reqBody, successCallback, errorCallback);
  }

  convertLayout(layoutObj: any, successCallback: (overlayObj: ConvertedOverlayApiDto) => void, errorCallback: () => void)
  {
    var reqBody = {RequestType: "ConvertLayoutToOverlay", LayoutConfig: layoutObj};
    this.executeAnonPostWithResponse<ConvertedOverlayApiDto>(reqBody, successCallback, errorCallback);
  }

  getLayout(layoutId: string, successCallback: (layoutObj: any) => void, errorCallback: () => void)
  {
    var reqBody = {RequestType: "GetLayout", GetLayout: {LayoutId:layoutId}};
    this.executeAuthPostWithResponse<any>(reqBody, successCallback, errorCallback);
  }

  getLayoutOverlay(layoutId: string, successCallback: (layoutObj: OverlayApiDto) => void, errorCallback: () => void)
  {
    let queryParams: string = "RequestType=GetLayoutOverlay&LayoutId=" + layoutId;
    this.executeAnonGetWithResponse<OverlayApiDto>(queryParams, successCallback, errorCallback);
  }

  getLayoutControls(layoutId: string, controlsIndex: number, successCallback: (response: LayoutControlsApiDto) => void, errorCallback: () => void)
  {
    var reqBody = {RequestType: "GetLayoutControls", GetLayoutControls: {LayoutId:layoutId,ControlsIndex:controlsIndex}};
    this.executeAuthPostWithResponse<LayoutControlsApiDto>(reqBody, successCallback, errorCallback);
  }

  getLayoutPropValues(layoutId: string, successCallback: (propValues: any[]) => void, errorCallback: () => void)
  {
    let queryParams: string = "RequestType=GetLayoutOverlayPropValues&LayoutId=" + layoutId;
    this.executeAnonGetWithResponse<any[]>(queryParams, successCallback, errorCallback);
  }

  createLayout(layoutName: string, successCallback: () => void, errorCallback: () => void)
  {
    var reqBody = {RequestType: "CreateLayout", CreateLayout: {Name:layoutName}};
    this.executeAuthPostNoResponse(reqBody, successCallback, errorCallback);
  }

  putLayout(layoutId: string, layoutConfigString: string, successCallback: () => void, errorCallback: () => void)
  {
    var reqBody = {RequestType: "PutLayout", PutLayout: {LayoutId:layoutId,LayoutConfig:JSON.parse(layoutConfigString)}};
    this.executeAuthPostNoResponse(reqBody, successCallback, errorCallback);
  }

  putLayoutPropertyValue(layoutId: string, propertyIndex: number, anyValue: any, successCallback: () => void, errorCallback: () => void)
  {
    var reqBody = {RequestType: "PutLayoutPropertyValues", PutLayoutPropertyValues: {LayoutId:layoutId,Values:[{PropertyIndex:propertyIndex,Value:anyValue}]}};
    this.executeAuthPostNoResponse(reqBody, successCallback, errorCallback);
  }
}
