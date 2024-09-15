import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, catchError } from "rxjs";
import { environment } from "src/environments/environment";
import { PagedResult } from '../models/paged.results.model';


@Injectable()

export class UserService {

  private apiUrl = environment.apiUrl + "/Users";

  constructor(private http: HttpClient) { }

  // Fetching all users
  getUserList(searchText: any, sortBy: any, sortOrder: any, pageIndex: any, pageSize: any): Observable<PagedResult<any>> {
    const userListEndPointUrl = `${this.apiUrl}/AllUsers?searchTerm=${searchText}'&sortBy=${sortBy}&sortOrder=${sortOrder}&page=${pageIndex}&pageSize=${pageSize}`;
    return this.http.get<PagedResult<any>>(userListEndPointUrl).pipe(
      catchError(this.handleError)
    );
  }

  // get user details by ID
  getUserById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  // Add User
  addUser(payload: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, payload).pipe(
      catchError(this.handleError)
    );
  }

  // update User 
  updateUser(id: number, item: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/UpdateUser?Id=${id}`, item).pipe(
      catchError(this.handleError)
    );
  }

  // Delete user by id

  deleteUser(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/DeleteUser?Id=${id}`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: any): Observable<never> {
    console.error('An error occurred', error);
    throw error;
  }
}