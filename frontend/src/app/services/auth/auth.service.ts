import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { LoginRequest } from '../../models/auth/login-request';
import { RegisterRequest } from '../../models/auth/register-request';
import { User } from '../../models/auth/user';
import { CookieService } from 'ngx-cookie-service';
import { VerifyEmail } from '../../models/auth/verify-email';
import { ApiResponse } from '../../models/api/api-response';
import { UserToken } from '../../models/auth/user-token';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private readonly tokenCookieKey = 'token';
  private readonly userCookieKey = 'currentUser';
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this.currentUserSubject = new BehaviorSubject<User | null>(this.getUserFromCookie());
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  register(request: RegisterRequest): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/register`, request);
  }

  verifyEmail(request: VerifyEmail): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/verify-email`, request);
  }

  resendVerificationCode(email: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(
      `${this.apiUrl}/send-resend-verification-code?userEmail=${email}`,
      {}
    );
  }

  login(request: LoginRequest): Observable<User> {
    return this.http.post<ApiResponse<UserToken>>(`${this.apiUrl}/login`, request).pipe(
      switchMap((response) => {
        const token = response.data?.token;
        if (!token) {
          return throwError(() => new Error(response.message ?? 'Missing authentication token'));
        }
        this.cookieService.set(this.tokenCookieKey, token, { expires: 1, path: '/' });
        return this.getCurrentUser(true);
      }),
      map((user) => {
        if (!user) {
          throw new Error('Unable to load current user after login');
        }
        return user;
      })
    );
  }

  logout(): void {
    this.cookieService.delete(this.userCookieKey, '/');
    this.cookieService.delete(this.tokenCookieKey, '/');
    this.currentUserSubject.next(null);
  }

  getCurrentUser(persistState = false): Observable<User | null> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/get-current-user`).pipe(
      map((response) => response.data ?? null),
      tap((user) => {
        if (user && persistState) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  hasToken(): boolean {
    return this.cookieService.check(this.tokenCookieKey);
  }

  private getUserFromCookie(): User | null {
    const user = this.cookieService.get(this.userCookieKey);
    if (user) {
      return JSON.parse(user);
    }
    return null;
  }

  private setCurrentUser(user: User): void {
    this.cookieService.set(this.userCookieKey, JSON.stringify(user), { expires: 1, path: '/' });
    this.currentUserSubject.next(user);
  }
}
