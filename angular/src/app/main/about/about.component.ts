import { Component } from '@angular/core';

import { environment } from '../../../environments/environment';

import { ApiService } from '../../api/api.service';
import { SettingsService } from '../../services/settings.service';

class SiteVars
{
  environment: string = "";
  
  version: string = "";
}

class ApiVars
{
  environment: string = "";
  
  version: string = "";
}

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent {

  siteVars: SiteVars | undefined;

  apiVars: ApiVars | undefined;

  constructor(private settingsService: SettingsService, private apiService: ApiService) {
    // Load site config
    this.settingsService.getConfig((config) =>
    {
      var siteVars = new SiteVars();
      siteVars.environment = config.environment;
      siteVars.version = environment.version;
      this.siteVars = siteVars;
    });
    // Load API config
    this.apiService.getInfo((response) =>
    {
      var apiVars = new ApiVars();
      apiVars.environment = response.Environment;
      apiVars.version = response.Version;
      this.apiVars = apiVars;
    }, () => { })
  }
}
