import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {

  config: any = null;

  constructor(private http: HttpClient)
  {
    this.http.get("./assets/config.json");
  }

  getApiUrl(callback: (apiUrl: string) => void)
  {
    // Check config is loaded
    var config = this.config;
    if (config !== null)
    {
      callback(config.apiUrl);
      return;
    }

    // Get config
    this.http.get<any>("./assets/config.json").subscribe(data => {
      this.config = data;
      callback(data.apiUrl);
    });
  }

  getCognitoClientId(callback: (cognitoClientId: string) => void)
  {
    // Check config is loaded
    var config = this.config;
    if (config !== null)
    {
      callback(config.cognitoClientId);
      return;
    }

    // Get config
    this.http.get<any>("./assets/config.json").subscribe(data => {
      this.config = data;
      callback(data.cognitoClientId);
    });
  }

  getCognitoClientIdAndCallbackUrl(callback: (cognitoClientId: string, cognitoCallbackUrl: string) => void)
  {
    // Check config is loaded
    var config = this.config;
    if (config !== null)
    {
      callback(config.cognitoClientId, config.cognitoCallbackUrl);
      return;
    }

    // Get config
    this.http.get<any>("./assets/config.json").subscribe(data => {
      this.config = data;
      callback(data.cognitoClientId, data.cognitoCallbackUrl);
    });
  }
}
