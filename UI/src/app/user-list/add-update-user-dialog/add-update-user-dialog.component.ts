import { Component, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { first } from 'rxjs';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/core/auth/auth.service';

@Component({
  selector: 'app-add-update-user-dialog',
  templateUrl: './add-update-user-dialog.component.html',
  styleUrls: ['./add-update-user-dialog.component.scss']
})
export class AddUpdateUserDialogComponent {
  form!: FormGroup;

  roles = [
    { Id: 1, RoleName: 'Admin' },
    { Id: 2, RoleName: 'User' },
  ];

  loading: boolean = false;
  isEdit: boolean = false;
  payload: any;
  userDetails: any;
  currentUser: any;

  constructor(
    public dialogRef: MatDialogRef<AddUpdateUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private userService: UserService,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) {
    this.currentUser = this.authService.loggedInUserDeatils.user;
    this.userFormInitilization();
    if (this.data.id) {
      // edit mode
      this.isEdit = true;
    }
  }

  ngOnInit() {
    if (this.data.id) {
      // edit mode
      this.isEdit = true;
      this.loading = true;

      this.userService.getUserById(this.data.id)
        .pipe(first())
        .subscribe(item => {
          this.form.controls['username'].disable();
          this.form.controls['password'].disable();
          let roles: any = [];
          this.userDetails = item;
          this.userDetails.userRoles.forEach((r: any) => {
            roles.push(r.roleId);
          });
          console.log(roles);
          this.form.patchValue(item);
          this.form.controls['roleIds'].setValue(roles);
          //  this.form.controls['roleIds'].setValue(item?.userRoles[0].roleId);
          this.loading = false;
        });
    }
  }

  userFormInitilization() {
    this.form = this.fb.group({
      id: [this.data.Id],
      firstName: ['', [Validators.required, Validators.maxLength(30)]],
      lastName: ['', Validators.maxLength(30)],
      username: [{ value: '', disabled: this.isEdit }, [Validators.required, Validators.maxLength(40)]],
      email: ['', [Validators.required, Validators.email]],
      password: [{ value: '', disabled: this.isEdit }, [Validators.required, Validators.minLength(8), Validators.maxLength(15)]],
      roleIds: ['', Validators.required],
      isActive: [true, Validators.required]
    });
  }
  onSubmit(): void {
    if (this.form.valid) {

      this.payload = this.form.value;
      //  let roleID = [this.form.controls['roleIds'].value]
      // this.form.controls['roleIds'].setValue(roleID);
      if (!this.data.id) {
        this.payload.id = 0;
        this.payload.createdBy = this.currentUser?.username;
        this.payload.updatedBy = this.currentUser?.username;
        this.userService.addUser(this.payload).subscribe((response) => {
          if (response.success) {
            this.snackBar.open(response?.message, 'Close', { duration: 2000, panelClass: 'my-custom-snackbar' });
            this.dialogRef.close(true);
          }
        });
      }
      else {
        this.payload.username = this.userDetails?.username;
        this.payload.password = "************";
        this.payload.updatedBy = this.currentUser?.username;
        this.userService.updateUser(this.data.id, this.form.value).subscribe((response) => {
          if (response.success) {
            this.dialogRef.close(true);
            this.snackBar.open(response?.message, 'Close', { duration: 2000, panelClass: 'my-custom-snackbar' });
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

}
