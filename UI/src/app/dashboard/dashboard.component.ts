import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

  userInfo: any;

  constructor(private router: Router) {
    this.userInfo = JSON.parse(localStorage.getItem('user')!);
  }

  isAdmin() {
    return  this.userInfo?.userRoles.map((r: any) => r).includes('Admin');
  }
  
}
