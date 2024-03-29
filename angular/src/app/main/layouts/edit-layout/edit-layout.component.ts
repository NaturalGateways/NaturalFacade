import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { SelectItem } from 'primeng/api';

import { ApiService } from '../../../api/api.service';

import { LoadLayoutService } from '../../../layout/services/load-layout.service';
import { LayoutData } from '../../../layout/layout-data';

import { RenderLayoutService } from '../../../layout/services/render-layout.service';

import { EditLayoutModel } from './edit-layout-model';

@Component({
  selector: 'app-edit-layout',
  templateUrl: './edit-layout.component.html',
  styleUrls: ['./edit-layout.component.css']
})
export class EditLayoutComponent {

  layoutId: string| null = null;

  layoutRender: RenderLayoutService | null = null;
  
  savedLayout: EditLayoutModel | null = null;
  
  previewLayout: EditLayoutModel | null = null;
  
  editedLayout: EditLayoutModel | null = null;
  
  typeOptions: SelectItem[];

  typeSelected: any;
  
  isBusy: boolean = true;

  isUpdateNeeded() { return this.previewLayout!.isEqual(this.editedLayout!) === false; }
  isSaveNeeded() { return this.isUpdateNeeded() === false && this.savedLayout!.isEqual(this.previewLayout!) === false; }

  constructor(private router: Router, private route: ActivatedRoute, private apiService: ApiService)
  {
    this.savedLayout = new EditLayoutModel(null, null, null);
    this.previewLayout = new EditLayoutModel(null, null, null);
    this.editedLayout = new EditLayoutModel(null, null, null);
    this.typeOptions = [
      { value:"Xml", label: "XML" },
      { value:"RawXml", label: "Raw XML" }
    ];
  }

  ngOnInit(): void {
    this.layoutRender = new RenderLayoutService(document.getElementById('overlayCanvas')!);

    this.route.queryParams.subscribe(params => {
      this.layoutId = params['layoutId'];
      this.apiService.getLayout(this.layoutId!, (loadedConfig) =>
      {
        this.savedLayout = new EditLayoutModel(loadedConfig, null, null);
        this.previewLayout = new EditLayoutModel(loadedConfig, null, null);
        this.editedLayout = new EditLayoutModel(loadedConfig, null, null);
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
    this.apiService.convertLayout(this.editedLayout!.createJsonObject(), (overlayObj) =>
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
        this.previewLayout = new EditLayoutModel(null, this.editedLayout, null);
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
    this.apiService.putLayout(this.layoutId!, this.previewLayout!.createJsonString(), () =>
    {
      this.router.navigate(['/layouts']);
      this.isBusy = false;
    }, () =>
    {
      this.isBusy = false;
    });
  }
}
