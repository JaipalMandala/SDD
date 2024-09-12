import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { HttpClient } from '@angular/common/http';
import { merge, Observable, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { UserService } from '../services/user.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AddUpdateUserDialogComponent } from './add-update-user-dialog/add-update-user-dialog.component';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationDialogComponent } from '../common/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent {

  dataSource = new MatTableDataSource<any>();
  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'username', 'email', 'role', 'isActive', 'actions'];
  currentLanguage: string = 'en';
  totalItems = 0;
  isLoadingResults = true;
  isRateLimitReached = false;


  orderBy: string = 'UpdatedDate';
  orderType: any = 'desc';
  pageSize: any = 5;
  pageIndex: any = 1;
  searchText: any = "";

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private userService: UserService, public dialog: MatDialog, private snackBar: MatSnackBar,
    private translateService: TranslateService,
  ) {
    this.translateService.onLangChange.subscribe(lang => {
      this.currentLanguage = lang.lang;
    });
  }

  ngOnInit(): void {
    this.loadItems(this.orderBy, this.orderType, this.pageSize, this.pageIndex);
  }

  onSortChange(event: any): void {
    this.orderBy = event.active;
    this.orderType = event.direction === 'asc';
    this.loadItems(this.orderBy, this.orderType, this.pageSize, this.pageIndex);
  }

  onPageChange(event: any): void {
    this.pageIndex = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadItems(this.orderBy, this.orderType, this.pageSize, this.pageIndex);
  }

  ngAfterViewInit() {
    setTimeout(() => {
      //  this.dataSource.sort = this.sort;
      // this.dataSource.paginator = this.paginator;
    }, 1000);
  }

  loadItems(orderBy: any, orderType: any, pageSize: any, pageIndex: any): any {
    this.userService.getUserList(this.searchText, orderBy, orderType, pageIndex, pageSize).subscribe(data => {
      data.items.map((u: any) => ({
        ...u,
        userRoles: this.transformUserRoles(u.userRoles, u),
      }))
      this.dataSource = new MatTableDataSource(data.items);
      this.totalItems = data.totalItems;
      this.pageSize = data.pageSize;
      this.pageIndex = data.page;
      this.dataSource.sort = this.sort;
      // this.dataSource.paginator = this.paginator;
      this.paginator.pageIndex = data.page - 1;
    });

  }

  transformUserRoles(userRoles: any, user: any) {
    const roles: any[] = userRoles.map((s: any) => ({
      roleName: s.role.roleName
    })); // no error

    const roleNames = Array.prototype.map.call(roles, s => s.roleName).toString();
    user.userRoles = roleNames;
    return roleNames;
  }

  openDialog(item?: any): void {
    const dialogRef = this.dialog.open(AddUpdateUserDialogComponent, { disableClose: true ,
      data: item || {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadItems(this.orderBy, this.orderType, this.pageSize, this.pageIndex);
      }
    });
  }

  deleteUser(id: number) {
    this.openConfirmationDialog(id);

  }

  openConfirmationDialog(id: number): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      height:'180px',
      data: { title: 'Delete', message: 'Are you sure you want to delete this user?', okText: 'Ok', cancelText: 'Cancel' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.deleteUser(id).subscribe((response) => {
          if (response.status) {
            this.loadItems(this.orderBy, this.orderType, this.pageSize, this.pageIndex);

            this.snackBar.open(response?.message, 'Close', {
              duration: 2000, panelClass: 'my-custom-snackbar', verticalPosition: 'top',
            });
          }
        });
      } else {
      }
    });
  }

  searchUser(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();    

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
}
