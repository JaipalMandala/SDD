import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthService } from './auth/auth.service';
import { ErrorInterceptor } from './interceptor/http-error.interceptor';


@NgModule({
  declarations: [
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [AuthService,ErrorInterceptor],
})
export class CoreModule { }
