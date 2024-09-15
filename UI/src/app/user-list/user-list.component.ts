import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
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

  userInfo: any;
  dataSource = new MatTableDataSource<any>();
  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'username', 'email', 'role', 'isActive', 'actions'];
  currentLanguage: string = 'en';
  totalItems = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  userList: any;
  orderBy: string = 'UpdatedDate';
  orderType: any = 'desc';
  pageSize: any = 5;
  pageIndex: any = 1;
  searchText: any = "";
  isLoading: boolean = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private userService: UserService, public dialog: MatDialog, private snackBar: MatSnackBar,
    private translateService: TranslateService,
    private matPaginatorIntl: MatPaginatorIntl
  ) {
    this.translateService.onLangChange.subscribe(lang => {
      this.currentLanguage = lang.lang;
      this.transalatePaginator(matPaginatorIntl);
    });
  }

  ngOnInit(): void {
    this.userInfo = JSON.parse(localStorage.getItem('user')!);
    this.loadItems(this.orderBy, this.orderType, this.pageSize, this.pageIndex);
  }

  ngAfterViewInit() {
    //  this.dataSource.sort = this.sort;
    // this.dataSource.paginator = this.paginator;
  }

  loadItems(orderBy: any, orderType: any, pageSize: any, pageIndex: any): any {
    this.userService.getUserList(this.searchText, orderBy, orderType, pageIndex, pageSize).subscribe(data => {
      data.items.map((u: any) => ({
        ...u,
        userRoles: this.transformUserRoles(u.userRoles, u),
      }))
      this.dataSource = new MatTableDataSource(data.items);
      this.userList = data.items;
      this.totalItems = data.totalItems;
      this.pageSize = data.pageSize;
      this.pageIndex = data.page;
      this.dataSource.sort = this.sort;
      // this.dataSource.paginator = this.paginator;
      this.paginator.pageIndex = data.page - 1;
      this.isLoading = true;
    }

    );
  }

  transformUserRoles(userRoles: any, user: any) {
    const roles: any[] = userRoles.map((s: any) => ({
      roleName: s.role.roleName
    }));

    const roleNames = Array.prototype.map.call(roles, s => s.roleName).toString();
    user.userRoles = roleNames;
    return roleNames;
  }

  hasRole(role: any) {
    return this.userInfo?.user?.userRoles.map((r: any) => r.role.roleName).includes(role);
  }

  private transalatePaginator(matPaginatorIntl: MatPaginatorIntl): void {
    matPaginatorIntl.itemsPerPageLabel = this.translateService.instant('PAGINATOR.ITEM');
    matPaginatorIntl.nextPageLabel = this.translateService.instant('PAGINATOR.NEXT_PAGE');
    matPaginatorIntl.previousPageLabel = this.translateService.instant('PAGINATOR.PREVIOUS_PAGE');
    matPaginatorIntl.firstPageLabel = this.translateService.instant('PAGINATOR.FIRST_PAGE');
    matPaginatorIntl.lastPageLabel = this.translateService.instant('PAGINATOR.LAST_PAGE');
    matPaginatorIntl.getRangeLabel = (page: number, pageSize: number, length: number) => {
      const of = this.translateService.instant('OF');
      if (length === 0 || pageSize === 0) {
        return `0 ${this.translateService.instant('OF')} ${length}`;
      }
      length = Math.max(length, 0);
      const startIndex = page * pageSize;
      const endIndex = Math.min(startIndex + pageSize, length);
      return `${startIndex + 1} - ${endIndex} ${of} ${length}`;
    }
    matPaginatorIntl.changes.next();
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

  openDialog(item?: any, actionType?: string): void {
    const dialogRef = this.dialog.open(AddUpdateUserDialogComponent, {
      disableClose: true,
      data: { item: item ? item : {}, isFormdisable: actionType === 'view' ? true : false } || {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.pageIndex = 1;
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
      height: '180px',
      data: { title: 'Delete', message: 'Are you sure you want to delete this user?', okText: 'Ok', cancelText: 'Cancel' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.deleteUser(id).subscribe((response) => {
          if (response.status) {
            this.loadItems(this.orderBy, this.orderType, this.pageSize, this.pageIndex);

            this.snackBar.open(response?.message, 'Close', {
              duration: 2000, panelClass: 'custom-snackbar',
            });
          }
        });
      } else {
      }
    });
  }

  searchUser(event: Event) {
    let filterValue = (event.target as HTMLInputElement).value;
    filterValue = filterValue.trim().toLowerCase();

    if (filterValue.length > 2) {
      const filteredUsers = this.dataSource.data.filter(user =>
        user.firstName.toLowerCase().includes(filterValue) || (user.lastName.toLowerCase()).includes(filterValue)
      );
      this.dataSource.data = filteredUsers;
    }
    else this.dataSource.data = this.userList;
    
  }
}
