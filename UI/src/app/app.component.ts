import { Component, Renderer2 } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'src/core/auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'user-management';
  userInfo: any;
  isValidUser: boolean = false;

  constructor(private authService: AuthService,
    public translateService: TranslateService,
    private renderer: Renderer2
  ) {
    this.translateService.onLangChange.subscribe(lang => {
      if (lang.lang === 'ar') {
        this.renderer.setAttribute(document.documentElement, 'dir', 'rtl');
      } else {
        this.renderer.removeAttribute(document.documentElement, 'dir');
      }
    });
  }

  ngOnInit() {
    this.userInfo = this.authService.loggedInUserDeatils;
    if (this.userInfo) this.isValidUser = true;
    else this.userInfo = false
  }
  changeLangage(lang?: any) {
    this.translateService.setDefaultLang(lang);
    this.translateService.use(lang);
  }
}
