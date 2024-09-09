import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
  
  userInfo: any;

 constructor(){
    this.userInfo = JSON.parse(localStorage.getItem('user')!);  
 }

 ngOnInit(){
  console.log(this.userInfo);
 }
}
