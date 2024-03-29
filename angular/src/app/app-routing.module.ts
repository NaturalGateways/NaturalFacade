import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './auth/login/login.component';

import { AdminComponent } from './admin/admin.component';
import { ViewCurrentUserComponent } from './admin/view-current-user/view-current-user.component';

import { RootComponent } from './root/root.component';
import { MainComponent } from './main/main.component';
import { AboutComponent } from './main/about/about.component';
import { DashboardComponent } from './main/dashboard/dashboard.component';

import { LayoutsComponent } from './main/layouts/layouts.component';
import { CreateLayoutComponent } from './main/layouts/create-layout/create-layout.component';
import { EditControlsComponent } from './main/layouts/edit-controls/edit-controls.component';
import { EditLayoutComponent } from './main/layouts/edit-layout/edit-layout.component';

import { ViewOverlayComponent } from './layout/view-overlay/view-overlay.component';

const routes: Routes = [
  { path: '', component: RootComponent, children:[
    { path: 'admin', component: AdminComponent, children:[
      { path: 'viewCurrentUser', component: ViewCurrentUserComponent }
    ]},
    { path: 'login', component:LoginComponent },
    { path: '', component: MainComponent, children:[
      { path: 'about', component: AboutComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'layouts', component: LayoutsComponent }
    ]},
    { path: 'layouts/create', component: CreateLayoutComponent },
    { path: 'layouts/controls/edit', component: EditControlsComponent },
    { path: 'layouts/edit', component: EditLayoutComponent }
  ]},
  { path: 'overlay', component: ViewOverlayComponent },
  { path: '**', component: MainComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
