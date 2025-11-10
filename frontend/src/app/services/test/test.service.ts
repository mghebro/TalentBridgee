import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTestRequest, Test } from '../../models/test/test';

@Injectable({
  providedIn: 'root'
})
export class TestService {
  private apiUrl = `${environment.apiUrl}/tests`;

  constructor(private http: HttpClient) { }

  getMyAssignedTests(): Observable<Test[]> {
    return this.http.get<Test[]>(`${this.apiUrl}/my-assignments`);
  }

  getTestById(id: string): Observable<Test> {
    return this.http.get<Test>(`${this.apiUrl}/${id}`);
  }

  createTest(test: CreateTestRequest): Observable<Test> {
    return this.http.post<Test>(this.apiUrl, test);
  }

  updateTest(id: string, test: CreateTestRequest): Observable<Test> {
    return this.http.put<Test>(`${this.apiUrl}/${id}`, test);
  }

  submitTest(testId: string, answers: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/${testId}/submit`, answers);
  }
}
