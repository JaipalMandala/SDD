import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/core/auth/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  userInfo: any;
  isValidUser: boolean = false;

  constructor(public authService: AuthService) { }


  ngOnInit() {
    this.userInfo = JSON.parse(localStorage.getItem('user')!);  
    if(this.userInfo)
    {
      this.isValidUser = true;
    }
   }

  logout() {
    this.authService.logout();
  }
}
