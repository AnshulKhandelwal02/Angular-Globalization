import { Component, OnInit } from '@angular/core';
import { TranslateService, LocaleModel } from './translate';

@Component({
  moduleId: module.id,
  selector: 'my-app',
  templateUrl: 'app.component.html',
})

export class AppComponent implements OnInit {

    public translatedText: string;
    public supportedLanguages: any[];
    public supportedLocales: LocaleModel[];
    public today: number;
    public pi: number;
    public value: number;

    public localizedDate: string;


    constructor(private _translate: TranslateService) {
        this.today = Date.now();
        this.pi = 3.14159;
        this.value = Math.round(Math.random() * 1000000) / 100;
    }

    ngOnInit() {
        // standing data

        this.supportedLocales = [
            { id:'en-US', display: 'United States', language: 'en', country:'US', numberPattern:'1.5-5', currency:'USD', currencyPattern:'1.2-2', currencySymbol: true, datePattern: 'mm/dd/yyyy' },
            { id:'es-SP', display: 'Spain', language: 'es', country:'SP', numberPattern:'1.2-2', currency:'EUR', currencyPattern:'1.1-1', currencySymbol: true, datePattern: 'fullDate' },
            { id:'en-GB', display: 'United Kingdom', language: 'en', country:'GB', numberPattern:'1.3-3', currency:'GBP', currencyPattern:'1.5-5', currencySymbol: true, datePattern: 'dd/mm/yyyy' },
            { id:'it-IT', display: 'Italy', language: 'it', country:'IT', numberPattern:'1.4-4', currency:'EUR', currencyPattern:'1.4-4', currencySymbol: true, datePattern: 'medium' },
            { id:'zh-CN', display: '华语', language: 'zh', country:'CN', numberPattern:'1.5-5', currency:'YEN', currencyPattern:'1.3-3', currencySymbol: true, datePattern: 'longdate' }
        ];

        this.subscribeToLangChanged(); // subscribe to language changes

        // set language
        //this._translate.setDefaultLang('en'); // set English as default
        this._translate.enableFallback(true); // enable fallback

        // set current langage
        //this.selectLang('es');

        // set current locale
        this.selectLang(this.supportedLocales[1]);

        this.localizeDate();
    }

    isCurrentLang(lang: string) {
        // check if the selected lang is current lang
        return lang === this._translate.currentLang;
    }

    // selectLang(lang: string) {
    //     // set current lang;
    //     this._translate.use(lang);
    //     //this.refreshText();
    // }

    selectLang(selectedLocale: LocaleModel) {
      this._translate.setCurrentLocale(selectedLocale);
    }

    refreshText() {
        // refresh translation when language change
        this.translatedText = this._translate.instant('hello world');
    }

    subscribeToLangChanged() {
        // refresh text
        // please unsubribe during destroy
        return this._translate.onLangChanged.subscribe((x:any) => this.refreshText());
    }

    subscribeToLocaleChanged() {
        return this._translate.onLocaleChanged.subscribe((x:any) => this.refreshText());
    }

    localizeDate() {
        this.localizedDate = this._translate.transformDate(this.today);
    }

}
