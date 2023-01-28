import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {

  config: any = null;

  constructor(private http: HttpClient)
  {
    //
  }

  getConfig(callback: (apiUrl: any) => void)
  {
    // Check config is loaded
    var config = this.config;
    if (config !== null)
    {
      callback(config);
      return;
    }

    // Get config
    this.http.get<any>("./assets/config.json").subscribe(data => {
      this.config = data;
      callback(data);
    });
  }

  getApiUrl(callback: (apiUrl: string) => void)
  {
    this.getConfig((data) =>
    {
      callback(data.apiUrl);
    });
  }

  getCognitoClientId(callback: (cognitoClientId: string) => void)
  {
    this.getConfig((data) =>
    {
      callback(data.cognitoClientId);
    });
  }

  getCognitoClientIdAndCallbackUrl(callback: (cognitoClientId: string, cognitoCallbackUrl: string) => void)
  {
    this.getConfig((data) =>
    {
      callback(data.cognitoClientId, data.cognitoCallbackUrl);
    });
  }
}
