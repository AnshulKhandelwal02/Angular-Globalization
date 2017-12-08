import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent }  from './app.component';
import { TRANSLATION_PROVIDERS, TranslatePipe, TranslateService,
  LocaleDatePipe, LocaleCurrencyPipe, LocaleDecimalPipe, LocalePercentPipe }   from './translate';


@NgModule({
  imports:      [ BrowserModule ],
  declarations: [
    AppComponent, TranslatePipe, LocaleDatePipe,
    LocaleCurrencyPipe, LocaleDecimalPipe, LocalePercentPipe
  ],
  bootstrap:    [ AppComponent ],
  providers:    [ TRANSLATION_PROVIDERS, TranslateService ]
})
export class AppModule { }
