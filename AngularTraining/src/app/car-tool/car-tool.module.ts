import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CarHomeComponent } from './components/car-home/car-home.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [CarHomeComponent],
  exports: [CarHomeComponent]
})
export class CarToolModule { }
