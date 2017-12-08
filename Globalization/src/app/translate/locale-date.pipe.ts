import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common'

import { TranslateService, LocaleModel } from '../translate'; // our translate service

@Pipe({
  name: 'localeDate',
  pure: true
})

export class LocaleDatePipe implements PipeTransform {

  // public _currentLocale: LocaleModel;

  constructor (private _localizationService: TranslateService) { }

  public transform(value: any): any {

      //this._currentLocale = this._localizationService.getCurrentLocale();

      // let localeDate: DatePipe = new DatePipe(this._currentLocale.language);
      // return localeDate.transform(value, this._currentLocale.datePattern);

      return this._localizationService.transformDate(value);
  }
}
