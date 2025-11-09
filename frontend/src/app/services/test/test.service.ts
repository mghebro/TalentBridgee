import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddQuestionRequest, AssignTestRequest, CreateTestRequest, GradeAnswerRequest, SubmitTestRequest, Test } from '../../models/test/test';
import { TestFilter } from '../../models/test/test-filter';
import { environment } from '../../../environments/environment';
import { PaginatedResult } from '../../models/paginated-result.model';

@Injectable({
  providedIn: 'root'
})
export class TestService {
  private apiUrl = environment.apiUrl + '/tests';
  private submissionAnswersApiUrl = environment.apiUrl + '/submission-answers';

  constructor(private http: HttpClient) { }

  createTest(request: CreateTestRequest): Observable<Test> {
    return this.http.post<Test>(this.apiUrl, request);
  }

  updateTest(id: number, request: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, request);
  }

  deleteTest(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  addQuestionToTest(testId: number, request: AddQuestionRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/${testId}/questions`, request);
  }

  assignTest(request: AssignTestRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/assign`, request);
  }

  startTestSubmission(testAssignmentId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${testAssignmentId}/start`, null);
  }

  submitTestSubmission(testSubmissionId: number, request: SubmitTestRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/${testSubmissionId}/submit`, request);
  }

  getTestById(id: number): Observable<Test> {
    return this.http.get<Test>(`${this.apiUrl}/${id}`);
  }

  getTests(filter: TestFilter): Observable<PaginatedResult<Test>> {
    let params = new HttpParams();
    if (filter.search) params = params.append('search', filter.search);
    if (filter.organizationId) params = params.append('organizationId', filter.organizationId.toString());
    if (filter.difficulty) params = params.append('difficulty', filter.difficulty);
    if (filter.page) params = params.append('page', filter.page.toString());
    if (filter.pageSize) params = params.append('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.append('sortBy', filter.sortBy);
    if (filter.sortOrder) params = params.append('sortOrder', filter.sortOrder);

    return this.http.get<PaginatedResult<Test>>(this.apiUrl, { params });
  }

  getTestSubmissionResult(testSubmissionId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${testSubmissionId}/result`);
  }

  gradeSubmissionAnswer(id: number, request: GradeAnswerRequest): Observable<any> {
    return this.http.put(`${this.submissionAnswersApiUrl}/${id}/grade`, request);
  }
}
