import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/core/auth/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {

  userInfo: any;
  user: any;

  constructor(public authService: AuthService) {
    this.authService.user.subscribe((x: any) => {
      this.user = x?.userRoles.includes('User');
      this.userInfo = x;
    });
  }

  ngOnInit() {
  }

  hasRole(role: any) {
    return this.userInfo?.user?.userRoles.map((r: any) => r.role.roleName).includes(role);
  }

  logout() {
    this.authService.logout();
  }
}
