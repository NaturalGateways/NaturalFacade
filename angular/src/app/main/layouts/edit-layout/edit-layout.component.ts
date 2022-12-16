import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { ApiService } from '../../../api/api.service';

import { LoadLayoutService } from '../../../layout/services/load-layout.service';
import { LayoutData } from '../../../layout/layout-data';

import { RenderLayoutService } from '../../../layout/services/render-layout.service';

@Component({
  selector: 'app-edit-layout',
  templateUrl: './edit-layout.component.html',
  styleUrls: ['./edit-layout.component.css']
})
export class EditLayoutComponent {

  layoutId: string| null = null;

  layoutRender: RenderLayoutService | null = null;
  
  savedLayoutJson: string | null = null;
  
  previewLayoutJson: string | null = null;
  
  editedLayoutJson: string | null = null;
  
  isBusy: boolean = true;

  constructor(private router: Router, private route: ActivatedRoute, private apiService: ApiService)
  {
    //
  }

  ngOnInit(): void {
    this.layoutRender = new RenderLayoutService(document.getElementById('overlayCanvas')!);

    this.route.queryParams.subscribe(params => {
      this.layoutId = params['layoutId'];
      this.apiService.getLayout(this.layoutId!, (loadedConfig) =>
      {
        this.savedLayoutJson = JSON.stringify(loadedConfig);
        this.editedLayoutJson = this.savedLayoutJson;
        this.isBusy = false;
      }, () =>
      {
        this.layoutRender!.loadingMessage = "Load Error.";
        this.layoutRender!.render(false);
        this.isBusy = false;
      });
    });
  }

  onPreviewLayout()
  {
    // Disable UI
    this.isBusy = true;

    // Do fetch
    this.layoutRender!.loadingMessage = "Fetching...";
    this.layoutRender!.render(false);
    this.apiService.convertLayout(JSON.parse(this.editedLayoutJson!), (overlayObj) =>
    {
      // Do fetch
      this.layoutRender!.loadingMessage = "Loading Resources...";
      this.layoutRender!.render(false);

      // Load data
      var loadLayoutService : LoadLayoutService = new LoadLayoutService();
      loadLayoutService.loadAllFromJson(overlayObj.overlay!, (loadedLayout) =>
      {
        this.layoutRender!.loadingMessage = "Success";
        this.layoutRender!.setPropValues(overlayObj.propValues);
        this.layoutRender!.setLayout(loadedLayout);
        this.layoutRender!.render(true);
        this.previewLayoutJson = this.editedLayoutJson;
        this.isBusy = false;
      }, () =>
      {
        this.layoutRender!.loadingMessage = "Load Error.";
        this.layoutRender!.render(false);
        this.isBusy = false;
      });
    }, () =>
    {
      this.layoutRender!.loadingMessage = "Fetch Errored.";
      this.layoutRender!.render(false);
      this.isBusy = false;
    });
  }

  onSaveLayout()
  {
    // Disable UI
    this.isBusy = true;

    // Do fetch
    this.apiService.putLayout(this.layoutId!, this.previewLayoutJson!, () =>
    {
      this.router.navigate(['/main/layouts']);
      this.isBusy = false;
    }, () =>
    {
      this.isBusy = false;
    });
  }
}
