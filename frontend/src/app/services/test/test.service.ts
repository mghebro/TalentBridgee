import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  CreateTestRequest,
  Test,
  SubmitAnswerRequest,
  SubmissionAnswer,
} from '../../models/test/test';
import { PaginatedResult, ServiceResult } from '../../models/api/api-response';

@Injectable({
  providedIn: 'root',
})
export class TestService {
  private apiUrl = `${environment.apiUrl}/tests`;

  constructor(private http: HttpClient) {}

  
  submitAnswer(
    assignmentId: number,
    questionId: number,
    request: SubmitAnswerRequest
  ): Observable<ServiceResult<SubmissionAnswer>> {
    const url = `${this.apiUrl}/assignments/${assignmentId}/questions/${questionId}/submit`;
    return this.http.post<ServiceResult<SubmissionAnswer>>(url, request);
  }

 
  handleQuestionTimeout(
    assignmentId: number,
    questionId: number
  ): Observable<ServiceResult<SubmissionAnswer>> {
    const url = `${this.apiUrl}/assignments/${assignmentId}/questions/${questionId}/timeout`;
    return this.http.post<ServiceResult<SubmissionAnswer>>(url, {});
  }

 

  getMyAssignedTests(): Observable<Test[]> {
    return this.http.get<ServiceResult<PaginatedResult<Test>>>(`${this.apiUrl}/my-assignments`).pipe(map((response) => response.data?.items ?? []));
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

  startTest(assignmentId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/assignments/${assignmentId}/start`, {});
  }

  submitTest(testId: string, answers: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/submissions/${testId}/submit`, answers);
  }

  completeTestSubmission(submissionId: number, payload: any = { answers: [] }): Observable<any> {
    return this.http.post(`${this.apiUrl}/submissions/${submissionId}/submit`, payload);
  }

  getTestSubmissionResult(submissionId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/submissions/${submissionId}/result`);
  }

  getTestsByOrganization(organizationId: number): Observable<Test[]> {
    const params = new HttpParams().append('organizationId', organizationId.toString());
    return this.http
      .get<ServiceResult<PaginatedResult<Test>>>(this.apiUrl, { params })
      .pipe(map((response) => response.data?.items ?? []));
  }
}
