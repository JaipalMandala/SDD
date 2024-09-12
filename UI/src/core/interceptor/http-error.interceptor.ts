import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { TranslateService } from "@ngx-translate/core";
import { catchError, Observable, throwError } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private snackBar: MatSnackBar,
        private translateService: TranslateService
    ) {

    }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        console.log('Http Request Started');
        return next.handle(req)
            .pipe(
                catchError((error: any) => {
                    console.log(error.error.message);
                    const errorMessage: any = this.setError(error);
                    this.snackBar.open(errorMessage?.message, 'Close', {
                        duration: 5000, verticalPosition: 'top',
                    });
                     return throwError(errorMessage);
                })
            );
    }

    setError(error: HttpErrorResponse): string{
        let errorMessage = 'Unknown error occured';
        if(error.error instanceof ErrorEvent){
            errorMessage = error?.error?.message;
        }
        else{
            if(error.status !== 0){
                errorMessage = error.error;
            }
        }
        return errorMessage;
    }
}