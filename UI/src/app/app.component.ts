import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'user-management';
  userInfo: any;
  isValidUser: boolean = false;

  constructor() {
    this.userInfo = JSON.parse(localStorage.getItem('user')!);  
    if(this.userInfo)
    {
      this.isValidUser = true;
    }
  }
}
