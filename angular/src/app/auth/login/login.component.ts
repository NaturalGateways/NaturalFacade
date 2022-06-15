import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";

import { CognitoService } from '../cognito.service';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit
{
  constructor(private router: Router, private route: ActivatedRoute, private cognitoService: CognitoService)
  {
    //
  }

  ngOnInit(): void
  {
    this.route.queryParams.subscribe(params => {
      this.cognitoService.authenticate(params['code'], ( ) : void => { this.router.navigate(['/']); });
    });
  }
}
