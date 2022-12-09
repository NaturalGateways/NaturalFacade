import { Component } from '@angular/core';

import { RenderLayoutService } from '../../../layout/services/render-layout.service';

@Component({
  selector: 'app-edit-layout',
  templateUrl: './edit-layout.component.html',
  styleUrls: ['./edit-layout.component.css']
})
export class EditLayoutComponent {

  layoutRender: RenderLayoutService | undefined;
  
  previewLayoutJson: string | null = null;
  
  editedLayoutJson: string | null = null;

  ngOnInit(): void {
    this.layoutRender = new RenderLayoutService(document.getElementById('overlayCanvas')!);
  }

  onPreviewLayout()
  {
    this.previewLayoutJson = this.editedLayoutJson;
  }
}
