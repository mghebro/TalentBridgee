import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { Login, LoginResponse } from '../models/auth/login';
import { Register } from '../models/auth/register';
import { User, UpdateUserResponse, UpdateUserRequest } from '../models/auth/user';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl + '/auth';
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User | null>(JSON.parse(localStorage.getItem('currentUser') || 'null'));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  register(registerData: Register): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, registerData).pipe(
      catchError(this.handleError)
    );
  }

  login(loginData: Login): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData)
      .pipe(
        tap(response => {
          if (response && response.token) {
            this.storeUserAndToken(response.user, response.token);
          }
        }),
        catchError(this.handleError)
      );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('jwtToken');
    this.currentUserSubject.next(null);
  }

  verifyEmail(email: string, code: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-email`, { email, code }).pipe(
      catchError(this.handleError)
    );
  }

  sendResendVerificationCode(email: string): Observable<any> {
    let params = new HttpParams().set('userEmail', email);
    return this.http.post(`${this.apiUrl}/send-resend-verification-code`, {}, { params }).pipe(
      catchError(this.handleError)
    );
  }

  sendResetPasswordLink(email: string): Observable<any> {
    let params = new HttpParams().set('userEmail', email);
    return this.http.post(`${this.apiUrl}/send-reset-password-link`, {}, { params }).pipe(
      catchError(this.handleError)
    );
  }

  resetPassword(resetPasswordData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/reset-password`, resetPasswordData).pipe(
      catchError(this.handleError)
    );
  }

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/get-current-user`).pipe(
      tap(user => {
        this.storeUserAndToken(user, this.getToken() || '');
      }),
      catchError(this.handleError)
    );
  }

  updateUser(userData: UpdateUserRequest): Observable<UpdateUserResponse> {
    return this.http.put<UpdateUserResponse>(`${this.apiUrl}/update-user`, userData).pipe(
      tap(response => {
        if (response && response.user) {
          this.storeUserAndToken(response.user, this.getToken() || '');
        }
      }),
      catchError(this.handleError)
    );
  }

  deleteUser(): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-user`).pipe(
      tap(() => this.logout()),
      catchError(this.handleError)
    );
  }

  private storeUserAndToken(user: User, token: string): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
    localStorage.setItem('jwtToken', token);
    this.currentUserSubject.next(user);
  }

  public getToken(): string | null {
    return localStorage.getItem('jwtToken');
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // Client-side errors
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side errors
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      if (error.error && error.error.message) {
        errorMessage = error.error.message;
      }
    }
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
