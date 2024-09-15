import { Component } from '@angular/core';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})

export class AdminComponent {

  userInfo: any;

  constructor() {
    this.userInfo = JSON.parse(localStorage.getItem('user')!);
  }
  
}
