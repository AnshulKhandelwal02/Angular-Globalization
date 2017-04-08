import { Pipe, PipeTransform } from '@angular/core';
import { CurrencyPipe } from '@angular/common';

import { TranslateService, LocaleModel } from '../translate'; // our translate service

@Pipe({
    name: 'localeCurrency',
    pure: true
})

export class LocaleCurrencyPipe implements PipeTransform {

  //public _currentLocale: LocaleModel;

  constructor (private _localizationService: TranslateService) { }

  public transform(value: any): any {

      //this._currentLocale = this._localizationService.getCurrentLocale();

      // let localeCurrency: CurrencyPipe = new CurrencyPipe(this._currentLocale.language);
      // return localeCurrency.transform(value,
      //     this._currentLocale.currency,
      //     this._currentLocale.currencySymbol,
      //     this._currentLocale.currencyPattern);

      return this._localizationService.transformCurrency(value);
  }
}

