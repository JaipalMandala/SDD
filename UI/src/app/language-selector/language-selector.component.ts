import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-language-selector',
  templateUrl: './language-selector.component.html',
  styleUrls: ['./language-selector.component.scss']
})
export class LanguageSelectorComponent {
  languages = [
    { code: 'en', name: 'En-US' },
    { code: 'ar', name: 'عربي' },
  ];

  currentLang!: string;

  constructor(private translateService: TranslateService ) {
    this.currentLang = this.translateService.currentLang;
  }

  changeLanguage(event: Event) {
    const selectedLang = (event.target as HTMLSelectElement).value;
    const browserLang = this.translateService.getBrowserLang();
    console.log(browserLang);
    this.translateService.use(browserLang?.match(/fr|fr-FR/) ? 'fr-FR' : 'en');
    this.translateService.use(selectedLang);
    this.currentLang = selectedLang;
  }
}
