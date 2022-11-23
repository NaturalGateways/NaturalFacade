import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import {ButtonModule} from 'primeng/button';
import {CardModule} from 'primeng/card';
import {ContextMenuModule} from 'primeng/contextmenu';
import {DataViewModule} from 'primeng/dataview';
import {MenubarModule} from 'primeng/menubar';
import {PanelModule} from 'primeng/panel';
import {TabMenuModule} from 'primeng/tabmenu';
import {TabViewModule} from 'primeng/tabview';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './main/main.component';
import { TestCanvasComponent } from './test-canvas/test-canvas.component';

import { LayoutModule } from './layout/layout.module';
import { LoginComponent } from './auth/login/login.component';
import { LayoutsComponent } from './main/layouts/layouts.component';
import { DashboardComponent } from './main/dashboard/dashboard.component';
import { AdminComponent } from './admin/admin.component';
import { ViewCurrentUserComponent } from './admin/view-current-user/view-current-user.component';

@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    TestCanvasComponent,
    LoginComponent,
    LayoutsComponent,
    DashboardComponent,
    AdminComponent,
    ViewCurrentUserComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    LayoutModule,
    ButtonModule,
    CardModule,
    ContextMenuModule,
    DataViewModule,
    MenubarModule,
    PanelModule,
    TabMenuModule,
    TabViewModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
