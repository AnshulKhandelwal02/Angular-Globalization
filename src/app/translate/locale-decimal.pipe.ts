import { Pipe, PipeTransform } from '@angular/core';
import { DecimalPipe } from '@angular/common';

import { TranslateService, LocaleModel } from '../translate'; // our translate service

@Pipe({
    name: 'localeDecimal',
    pure: true
})

export class LocaleDecimalPipe implements PipeTransform {

  // public _currentLocale: LocaleModel;

  constructor (private _localizationService: TranslateService) { }

  public transform(value: any): any {

      // this._currentLocale = this._localizationService.getCurrentLocale();

      // let localeDecimal: DecimalPipe = new DecimalPipe(this._currentLocale.language);
      // return localeDecimal.transform(value, this._currentLocale.numberPattern)

      return this._localizationService.transformNumber(value);
  }

}

