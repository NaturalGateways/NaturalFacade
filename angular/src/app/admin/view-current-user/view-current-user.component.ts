import { Component } from '@angular/core';

import { ApiService } from '../../api/api.service';

@Component({
  selector: 'app-view-current-user',
  templateUrl: './view-current-user.component.html',
  styleUrls: ['./view-current-user.component.css']
})
export class ViewCurrentUserComponent {

  username: string = "Alpha";
  email: string = "Beta";

  constructor(private apiService: ApiService)
  {
    this.username = apiService.apiAuthModel?.user.name!;
    this.email = apiService.apiAuthModel?.user.email!;
  }
}
