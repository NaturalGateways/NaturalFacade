import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './main/main.component';
import { TestCanvasComponent } from './test-canvas/test-canvas.component';

import { LayoutModule } from './layout/layout.module';

@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    TestCanvasComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    LayoutModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
