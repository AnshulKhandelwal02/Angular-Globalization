// app/translate/translate.service.ts

import { Injectable, Inject, EventEmitter } from '@angular/core';
import { DatePipe, CurrencyPipe, DecimalPipe, PercentPipe } from "@angular/common";
import { TRANSLATIONS } from './translations'; // import our opaque token
import { LocaleModel } from './locale.model';

@Injectable()
export class TranslateService {
    private PLACEHOLDER = '%'; // our placeholder

    private _currentLang: string;
    private _defaultLang: string;
    private _fallback: boolean;
    private _defaultLocale: LocaleModel;
    private _currentLocale: LocaleModel;

    // inject our translations
    constructor(@Inject(TRANSLATIONS) private _translations: any) {
    }


    // add event
    public onLangChanged: EventEmitter<string> = new EventEmitter<string>();
    public onLocaleChanged: EventEmitter<LocaleModel> = new EventEmitter<LocaleModel>();

    public get currentLang() {
        return this._currentLang || this._defaultLang;;
    }

    public setDefaultLang(lang: string) {
        this._defaultLang = lang; // set default lang
    }

    public setDefaultLocale(defaultLocale: LocaleModel ) {
        this._defaultLocale = defaultLocale;
    }

    public getCurrentLocale() {
        return this._currentLocale || this._defaultLocale;
    }

    public enableFallback(enable: boolean) {
        this._fallback = enable; // enable or disable fallback language
    }

    public setCurrentLocale(currentLocale: LocaleModel) {
      this._currentLocale = currentLocale;
      this.onLangChanged.emit(this._currentLocale.language);
      this.onLocaleChanged.emit(this._currentLocale);
    }

    public use(lang: string): void {
        // set current language
        this._currentLang = lang;
        this.onLangChanged.emit(lang); // publish changes
    }

    private translate(key: string): string { // refactor our translate implementation
        let translation = key;

        // found in current language
        if (this._translations[this.currentLang] && this._translations[this.currentLang][key]) {
            return this._translations[this.currentLang][key];
        }

        // fallback disabled
        if (!this._fallback) {
            return translation;
        }

        // found in default language
        if (this._translations[this._defaultLang] && this._translations[this._defaultLang][key]) {
            return this._translations[this._defaultLang][key];
        }

        // not found
        return translation;
    }

    public instant(key: string, words?: string | string[]) { // add optional parameter
        const translation: string = this.translate(key);

        if (!words) return translation;
        return this.replace(translation, words); // call replace function
    }

    public replace(word: string = '', words: string | string[] = '') {
        let translation: string = word;

        const values: string[] = [].concat(words);
        values.forEach((e, i) => {
            translation = translation.replace(this.PLACEHOLDER.concat(<any>i), e);
        });

        return translation;
    }

    public transformDate(value: any) {
      let localeDate: DatePipe = new DatePipe(this._currentLocale.language);
      return localeDate.transform(value, this._currentLocale.datePattern);
    }

    public transformCurrency(value: any) {
      let localeCurrency: CurrencyPipe = new CurrencyPipe(this._currentLocale.language);
      return localeCurrency.transform(value,
          this._currentLocale.currency,
          this._currentLocale.currencySymbol,
          this._currentLocale.currencyPattern);
    }

    public transformNumber(value: any) {
      let localeDecimal: DecimalPipe = new DecimalPipe(this._currentLocale.language);
      return localeDecimal.transform(value, this._currentLocale.numberPattern)
    }

}
