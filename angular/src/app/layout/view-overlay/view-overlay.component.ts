import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ApiService } from '../../api/api.service';

import { RenderLayoutService } from '../services/render-layout.service';
import { LoadLayoutService } from '../services/load-layout.service';

@Component({
  selector: 'app-view-overlay',
  templateUrl: './view-overlay.component.html',
  styleUrls: ['./view-overlay.component.css']
})
export class ViewOverlayComponent {

  layoutId: string | undefined;

  layoutRender: RenderLayoutService | undefined;

  apiFetchMillisLeft: number = 0;

  constructor(private route: ActivatedRoute, private apiService: ApiService)
  {
    //
  }

  ngOnInit(): void {
    // Get layout render
    this.layoutRender = new RenderLayoutService(document.getElementById('overlayCanvas')!);
    this.layoutRender.loadingBackColour = "#003366";
    this.layoutRender.loadingMessage = "Loading...";
    this.layoutRender.render(false);

    this.route.queryParams.subscribe(params => {
      // Get params 
      this.layoutId = params['layoutId'];

      // Fetch from API
      this.layoutRender!.loadingMessage = "Fetching...";
      this.layoutRender!.render(false);
      this.apiService.getLayoutPropValues(this.layoutId!, (propValues) =>
      {
        this.apiService.getLayoutOverlay(this.layoutId!, (layoutOverlay) =>
        {
          // Set canvas size
          this.layoutRender!.canvas.width = layoutOverlay.canvasSize[0];
          this.layoutRender!.canvas.height = layoutOverlay.canvasSize[1];
  
          // Load layout
          this.layoutRender!.loadingMessage = "Loading Resources...";
          this.layoutRender!.render(false);
          var loadLayoutService : LoadLayoutService = new LoadLayoutService();
          loadLayoutService.loadAllFromJson(layoutOverlay, (loadedLayout) =>
          {
            this.layoutRender!.loadingMessage = "Success";
            this.layoutRender!.setPropValues(propValues);
            this.layoutRender!.setLayout(loadedLayout);
            this.layoutRender!.updateOverlay();
            this.layoutRender!.render(true);

            if (layoutOverlay.redrawMillis !== null && layoutOverlay.apiFetchMillis !== null)
            {
              this.apiFetchMillisLeft = layoutOverlay.apiFetchMillis;
              setInterval(() => {
                if (this.apiFetchMillisLeft < layoutOverlay.redrawMillis!)
                {
                  this.onFetch();
                  this.apiFetchMillisLeft = layoutOverlay.apiFetchMillis!;
                }
                else
                {
                  this.layoutRender!.render(true);
                  this.apiFetchMillisLeft = this.apiFetchMillisLeft - layoutOverlay.redrawMillis!;
                }
              }, layoutOverlay.redrawMillis);
            }
            else if (layoutOverlay.redrawMillis !== null)
            {
              setInterval(() => {
                this.layoutRender!.render(true);
              }, layoutOverlay.redrawMillis);
            }
            else if (layoutOverlay.apiFetchMillis !== null)
            {
              setInterval(() => { this.onFetch(); }, layoutOverlay.apiFetchMillis);
            }
          }, () =>
          {
            this.layoutRender!.loadingMessage = "Load Error.";
            this.layoutRender!.render(false);
          });
        }, () =>
        {
          this.layoutRender!.loadingMessage = "Load Error.";
          this.layoutRender!.render(false);
        });
      }, () =>
      {
        this.layoutRender!.loadingMessage = "Load Error.";
        this.layoutRender!.render(false);
      });
    });
  }

  onFetch()
  {
    this.apiService.getLayoutPropValues(this.layoutId!, (propValues) =>
    {
      this.layoutRender!.setPropValues(propValues);
      this.layoutRender!.updateOverlay();
      this.layoutRender!.render(true);
    }, () => { });
  }
}
