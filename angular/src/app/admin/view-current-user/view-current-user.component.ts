import { Component } from '@angular/core';
import { timeStamp } from 'console';

import { ApiService } from '../../api/api.service';
import { CognitoService, CognitoServiceAuthStatus } from '../../auth/cognito.service';

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

  constructor(private cognitoService: CognitoService, private apiService: ApiService)
  {
    this.username = this.cognitoService.apiAuthModel?.user.Name!;
    this.email = this.cognitoService.apiAuthModel?.user.Email!;
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
      this.username = userResponse.Name!;
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
