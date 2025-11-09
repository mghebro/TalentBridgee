import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Register } from '../../models/auth/register';
import { Login } from '../../models/auth/login';
import { User } from '../../models/auth/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api/auth';

  constructor(private http: HttpClient) { }

  register(data: Register): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  login(data: Login): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, data);
  }

  verifyEmail(data: any): Observable<any> {
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
    return this.http.get<User>(`${this.apiUrl}/get-current-user`);
  }

  updateUser(data: User): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-user`, data);
  }

  deleteUser(): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-user`);
  }
}
