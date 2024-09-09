import { Component } from '@angular/core';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { first } from 'rxjs';
import { AuthService } from 'src/core/auth/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  logInForm!: FormGroup;
  loading: boolean = false;
  submitted: boolean = false;
  error?: any;


  constructor(private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService) {
    if (this.authService.userValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit() {
    this.logInForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    })
  }

  get f() { return this.logInForm.controls; }

  onSubmit() {
    this.submitted = true;

    this.error = '';

    if (this.logInForm.invalid) {
      return;
    }

    this.loading = false;

    const { username, password } = this.logInForm.value;

    this.authService.login(username, password)
      .pipe(first())
      .subscribe({
        next: () => {
          this.loading = true;
          this.router.navigateByUrl('/dashboard');
        },
        error: error => {
          this.error = error;
          this.loading = false;
          return false;
        }
      });
  }
}
