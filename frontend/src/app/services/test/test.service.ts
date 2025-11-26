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
    return this.http
      .get<ServiceResult<PaginatedResult<Test>>>(`${this.apiUrl}/my-assignments`)
      .pipe(map((response) => response.data?.items ?? []));
  }

  getMyCreatedTests(): Observable<Test[]> {
    return this.http
      .get<ServiceResult<PaginatedResult<Test>>>(`${this.apiUrl}/my-tests`)
      .pipe(map((response) => response.data?.items ?? []));
  }

  getTestById(id: string): Observable<Test> {
    return this.http
      .get<ServiceResult<Test>>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => this.unwrapResult(response, 'Failed to load test')));
  }

  createTest(test: CreateTestRequest): Observable<Test> {
    return this.http
      .post<ServiceResult<Test>>(this.apiUrl, test)
      .pipe(map((response) => this.unwrapResult(response, 'Failed to create test')));
  }

  private unwrapResult<T>(response: ServiceResult<T>, fallbackMessage: string): T {
    if (!response.data) {
      throw new Error(response.message ?? response.errors?.[0] ?? fallbackMessage);
    }
    return response.data;
  }

  updateTest(id: string, test: CreateTestRequest): Observable<Test> {
    return this.http
      .put<ServiceResult<Test>>(`${this.apiUrl}/${id}`, test)
      .pipe(map((response) => this.unwrapResult(response, 'Failed to update test')));
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
