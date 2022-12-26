import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import {ButtonModule} from 'primeng/button';
import {CardModule} from 'primeng/card';
import {ContextMenuModule} from 'primeng/contextmenu';
import {DataViewModule} from 'primeng/dataview';
import {DividerModule} from 'primeng/divider';
import {InputTextareaModule} from 'primeng/inputtextarea';
import {InputTextModule} from 'primeng/inputtext';
import {MenubarModule} from 'primeng/menubar';
import {PanelModule} from 'primeng/panel';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
import {TabMenuModule} from 'primeng/tabmenu';
import {TabViewModule} from 'primeng/tabview';
import {ToggleButtonModule} from 'primeng/togglebutton';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './main/main.component';

import { LayoutModule } from './layout/layout.module';
import { LoginComponent } from './auth/login/login.component';
import { LayoutsComponent } from './main/layouts/layouts.component';
import { DashboardComponent } from './main/dashboard/dashboard.component';
import { AdminComponent } from './admin/admin.component';
import { ViewCurrentUserComponent } from './admin/view-current-user/view-current-user.component';
import { CreateLayoutComponent } from './main/layouts/create-layout/create-layout.component';
import { EditLayoutComponent } from './main/layouts/edit-layout/edit-layout.component';
import { LayoutGridItemComponent } from './main/layouts/layout-item/layout-grid-item/layout-grid-item.component';
import { LayoutListItemComponent } from './main/layouts/layout-item/layout-list-item/layout-list-item.component';
import { EditControlsComponent } from './main/layouts/edit-controls/edit-controls.component';
import { EditControlsFieldComponent } from './main/layouts/edit-controls/edit-controls-field/edit-controls-field.component';
import { RootComponent } from './root/root.component';

@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    LoginComponent,
    LayoutsComponent,
    DashboardComponent,
    AdminComponent,
    ViewCurrentUserComponent,
    CreateLayoutComponent,
    EditLayoutComponent,
    LayoutGridItemComponent,
    LayoutListItemComponent,
    EditControlsComponent,
    EditControlsFieldComponent,
    RootComponent
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
    InputTextareaModule,
    InputTextModule,
    MenubarModule,
    PanelModule,
    ProgressSpinnerModule,
    TabMenuModule,
    TabViewModule,
    ToggleButtonModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
