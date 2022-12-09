import { Component } from '@angular/core';

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

  layoutRender: RenderLayoutService | null = null;
  
  previewLayoutJson: string | null = null;
  
  editedLayoutJson: string | null = null;
  
  isFetching: boolean = false;

  constructor(private apiService: ApiService)
  {
    //
  }

  ngOnInit(): void {
    this.layoutRender = new RenderLayoutService(document.getElementById('overlayCanvas')!);
  }

  onPreviewLayout()
  {
    // Disable UI
    this.isFetching = true;
    this.previewLayoutJson = this.editedLayoutJson;

    // Do fetch
    this.layoutRender!.loadingMessage = "Fetching...";
    this.layoutRender!.render(false);
    this.apiService.convertLayout(JSON.parse(this.editedLayoutJson!), (overlayObj) =>
    {
      // Do fetch
      this.layoutRender!.loadingMessage = "Loading Resources...";

      // Load data
      var loadLayoutService : LoadLayoutService = new LoadLayoutService();
      loadLayoutService.loadAllFromJson(overlayObj, (loadedLayout) =>
      {
        this.layoutRender!.loadingMessage = "Success";
        this.layoutRender!.setLayout(loadedLayout);
        this.layoutRender!.render(true);
        this.isFetching = false;
      }, () =>
      {
        this.layoutRender!.loadingMessage = "Load Error.";
        this.isFetching = false;
      });
    }, () =>
    {
      this.layoutRender!.loadingMessage = "Fetch Errored.";
      this.isFetching = false;
    });
  }
}
