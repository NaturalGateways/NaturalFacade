import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import {ButtonModule} from 'primeng/button';
import {CardModule} from 'primeng/card';
import {ContextMenuModule} from 'primeng/contextmenu';
import {DataViewModule} from 'primeng/dataview';
import {DividerModule} from 'primeng/divider';
import {InputTextModule} from 'primeng/inputtext';
import {MenubarModule} from 'primeng/menubar';
import {PanelModule} from 'primeng/panel';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
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
import { CreateLayoutComponent } from './main/layouts/create-layout/create-layout.component';

@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    TestCanvasComponent,
    LoginComponent,
    LayoutsComponent,
    DashboardComponent,
    AdminComponent,
    ViewCurrentUserComponent,
    CreateLayoutComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
    LayoutModule,
    ButtonModule,
    CardModule,
    ContextMenuModule,
    DataViewModule,
    DividerModule,
    InputTextModule,
    MenubarModule,
    PanelModule,
    ProgressSpinnerModule,
    TabMenuModule,
    TabViewModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
