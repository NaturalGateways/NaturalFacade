import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './auth/login/login.component';

import { DashboardComponent } from './main/dashboard/dashboard.component';
import { MainComponent } from './main/main.component';
import { LayoutsComponent } from './main/layouts/layouts.component';
import { TestCanvasComponent } from './test-canvas/test-canvas.component';

const routes: Routes = [
  { path: 'testcanvas', component: TestCanvasComponent },
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
