import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './auth/login/login.component';

import { AdminComponent } from './admin/admin.component';
import { ViewCurrentUserComponent } from './admin/view-current-user/view-current-user.component';

import { MainComponent } from './main/main.component';
import { DashboardComponent } from './main/dashboard/dashboard.component';

import { LayoutsComponent } from './main/layouts/layouts.component';
import { TestCanvasComponent } from './test-canvas/test-canvas.component';

const routes: Routes = [
  { path: 'testcanvas', component: TestCanvasComponent },
  { path: 'admin', component: AdminComponent, children:[
    { path: 'viewCurrentUser', component: ViewCurrentUserComponent }
  ]},
  { path: 'login', component:LoginComponent },
  { path: 'main', component: MainComponent, children:[
    { path: 'dashboard', component: DashboardComponent },
    { path: 'layouts', component: LayoutsComponent }
  ]},
  { path: '**', component: MainComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
