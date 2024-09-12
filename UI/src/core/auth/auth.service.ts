import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from 'src/environments/environment';
import { User } from 'src/app/models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private userSubject: BehaviorSubject<any | null>;
    public user: Observable<any | null>;

    constructor(
        private router: Router,
        private http: HttpClient
    ) {
        this.userSubject = new BehaviorSubject(JSON.parse(localStorage.getItem('user')!));
        this.user = this.userSubject.asObservable();
    }

    public get loggedInUserDeatils() {
        if (!this.userSubject.value) {
            this.userSubject = new BehaviorSubject(JSON.parse(localStorage.getItem('user')!))
        }
        return this.userSubject.value;
    }

    login(username: string, password: string) {
        return this.http.post<any>(`${environment.apiUrl}/Auth/login`, { username, password })
            .pipe(map(user => {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                localStorage.setItem('user', JSON.stringify(user));
                this.userSubject.next(user);
                return user;
            }));
    }

    logout() {
        // remove user from local storage and set current user to null
        localStorage.removeItem('user');
        this.userSubject.next(null);
        this.router.navigate(['/login']);
    }

    hasRole(role: string): boolean {
        const user = this.loggedInUserDeatils;
        if (user && user?.userRoles) {
            return user?.userRoles.includes(role);
        }
        return false;
    }
}