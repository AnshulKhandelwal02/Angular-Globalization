import { Pipe, PipeTransform } from '@angular/core';
import { PercentPipe } from '@angular/common';

import { TranslateService, LocaleModel } from '../translate'; // our translate service

@Pipe({
    name: 'localePercent',
    pure: true
})

export class LocalePercentPipe implements PipeTransform {

  public _currentLocale: LocaleModel;

  constructor (private _localizationService: TranslateService) { }

  public transform(value: any): any {

      this._currentLocale = this._localizationService.getCurrentLocale();

      let localePercent: PercentPipe = new PercentPipe(this._currentLocale.language);
      return localePercent.transform(value, this._currentLocale.numberPattern);
  }
}

