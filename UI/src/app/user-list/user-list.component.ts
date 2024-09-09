import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AddUpdateUserDialogComponent } from './add-update-user-dialog/add-update-user-dialog.component';
import { UserTest } from '../models/user.model';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent {

  dataSource: any[] = [];
  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'username', 'email', 'role', 'isActive', 'actions'];

  constructor(private userService: UserService, public dialog: MatDialog, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems() {
    this.userService.getUserList().subscribe(data => {
      this.dataSource = data;
    });
  }

  openDialog(item?: any): void {
    const dialogRef = this.dialog.open(AddUpdateUserDialogComponent, {
      width: '300px',
      height: '600px',
      data: item || {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.id) {
          this.userService.updateUser(result.id, result).subscribe(() => {
            this.loadItems();
            this.snackBar.open('Item updated!', 'Close', { duration: 2000 });
          });
        } else {
          this.userService.addUser(result).subscribe(() => {
            this.loadItems();
            this.snackBar.open('Item added!', 'Close', { duration: 2000 });
          });
        }
      }
    });
  }

  deleteUser(id: number) {
    this.userService.deleteUser(id).subscribe((response) => {
      if (response.status) {
        this.loadItems();
        this.snackBar.open(response?.message, 'Close', { duration: 2000, panelClass: 'my-custom-snackbar' });
      }
    });
  }
}
