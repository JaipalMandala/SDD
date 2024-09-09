import { Component, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-add-update-user-dialog',
  templateUrl: './add-update-user-dialog.component.html',
  styleUrls: ['./add-update-user-dialog.component.scss']
})
export class AddUpdateUserDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<AddUpdateUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private userService: UserService
  ) {
    this.form = this.fb.group({
      id: [data.id],
      firstName: [data.firstName || '', Validators.required],
      lastName: [data.lastName || ''],
      username: [data.username || '', Validators.required],
      email: [data.email || '', [Validators.required, Validators.email]],
      role: [data.role || '', [Validators.required]],
      isActive: [data.isActive]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.userService.addUser(this.form.value).subscribe((response) => {
        if(response.status){
          this.dialogRef.close(this.form.value);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

}
