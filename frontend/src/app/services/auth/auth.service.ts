import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Register } from '../../models/auth/register';
import { Login } from '../../models/auth/login';
import { User } from '../../models/auth/user';
import { VerifyEmail } from '../../models/auth/verify-email';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:5163/api/auth';
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this.currentUserSubject = new BehaviorSubject<User | null>(this.getUserFromCookie());
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  private getUserFromCookie(): User | null {
    const user = this.cookieService.get('user');
    return user ? JSON.parse(user) : null;
  }

  register(data: any): Observable<any> {
  return this.http.post(`${this.apiUrl}/register`, data);
}
  login(data: Login): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, data).pipe(
      tap((response) => {
        if (response && response.token) {
          this.cookieService.set('jwtToken', response.token);
          // Assuming the user object is returned in the response
          const user: User = response.user;
          this.cookieService.set('user', JSON.stringify(user));
          this.currentUserSubject.next(user);
        }
      })
    );
  }

  logout() {
    this.cookieService.delete('jwtToken');
    this.cookieService.delete('user');
    this.currentUserSubject.next(null);
  }

  verifyEmail(data: VerifyEmail): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-email`, data);
  }

  sendResendCode(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/send-resend-verification-code?userEmail=${email}`, null);
  }

  sendResetPasswordLink(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/send-reset-password-link?userEmail=${email}`, null);
  }

  resetPassword(data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/reset-password`, data);
  }

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/get-current-user`).pipe(
      tap(user => {
        this.cookieService.set('user', JSON.stringify(user));
        this.currentUserSubject.next(user);
      })
    );
  }

  updateUser(data: User): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-user`, data);
  }

  deleteUser(): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-user`);
  }
}
