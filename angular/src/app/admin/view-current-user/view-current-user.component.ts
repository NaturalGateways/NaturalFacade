import { Component } from '@angular/core';
import { timeStamp } from 'console';

import { ApiService } from '../../api/api.service';

@Component({
  selector: 'app-view-current-user',
  templateUrl: './view-current-user.component.html',
  styleUrls: ['./view-current-user.component.css']
})
export class ViewCurrentUserComponent {

  username: string = "";
  email: string = "";

  usernameEditState: number = 0;
  usernameEdit: string = "";

  constructor(private apiService: ApiService)
  {
    this.username = this.apiService.apiAuthModel?.user.Name!;
    this.email = this.apiService.apiAuthModel?.user.Email!;
  }

  onEditUsername()
  {
    this.usernameEdit = this.username;
    this.usernameEditState = 1;
  }

  onSaveUsername()
  {
    this.usernameEditState = 2;
    this.apiService.updateCurrentUser(this.usernameEdit, (userResponse) =>
    {
      this.username = this.usernameEdit;
      this.usernameEditState = 0;
    }, () =>
    {
      this.usernameEditState = 1;
    });
  }

  onCancelUsername()
  {
    this.usernameEditState = 0;
  }
}
