import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { LoginRequest } from '../../models/auth/login-request';
import { LoginResponse } from '../../models/auth/login-response.model';
import { RegisterRequest } from '../../models/auth/register-request';
import { User } from '../../models/auth/user';
import { CookieService } from 'ngx-cookie-service';
import { VerifyEmail } from '../../models/auth/verify-email';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this.currentUserSubject = new BehaviorSubject<User | null>(this.getUserFromCookie());
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, request);
  }

  verifyEmail(request: VerifyEmail): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-email`, request);
  }

  resendVerificationCode(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/send-resend-verification-code?userEmail=${email}`, {});
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        if (response && response.token) {
          this.cookieService.set('currentUser', JSON.stringify(response.user), { expires: 1, path: '/' });
          this.cookieService.set('token', response.token, { expires: 1, path: '/' });
          this.currentUserSubject.next(response.user);
        }
      })
    );
  }

  logout(): void {
    this.cookieService.delete('currentUser', '/');
    this.cookieService.delete('token', '/');
    this.currentUserSubject.next(null);
  }

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/get-current-user`);
  }

  private getUserFromCookie(): User | null {
    const user = this.cookieService.get('currentUser');
    if (user) {
      return JSON.parse(user);
    }
    return null;
  }
}