export class LocaleModel {
  id: string;
  display: string;
  language: string;
  country: string;
  numberPattern: string;
  datePattern: string;
  currency: string;
  currencyPattern: string;
  currencySymbol: boolean;

  localeFormat?: string = this.language + '-' + this.country;
}

