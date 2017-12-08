import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import { CarToolModule } from './car-tool/car-tool.module';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule, CarToolModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
