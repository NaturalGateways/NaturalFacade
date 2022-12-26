import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ApiService } from '../../../api/api.service';

@Component({
  selector: 'app-create-layout',
  templateUrl: './create-layout.component.html',
  styleUrls: ['./create-layout.component.css']
})
export class CreateLayoutComponent {

  layoutName: string = "";

  isSubmitting: boolean = false;
  
  layoutNameError: string | null = null;
  submitError: string | null = null;

  constructor(private apiService: ApiService, private router: Router)
  {
    //
  }

  onSaveLayout()
  {
    // Set submitting
    this.isSubmitting = true;
    this.layoutNameError = null;
    this.submitError = null;
    
    // Check layout name
    if (this.layoutName.length <= 0)
    {
      this.layoutNameError = "Please enter a name.";
      this.isSubmitting = false;
      return;
    }

    // Send layout
    this.apiService.createLayout(this.layoutName, () =>
    {
      this.router.navigate(['/layouts']);
      this.isSubmitting = false;
    }, () =>
    {
      this.submitError = "Send error.";
      this.isSubmitting = false;
    });
  }
}
