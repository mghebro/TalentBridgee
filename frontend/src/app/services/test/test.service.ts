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
    return this.http.get<ServiceResult<any>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        const test = this.unwrapResult(response, 'Failed to load test');
        if (test.questions || test.Questions) {
          const questions = test.questions || test.Questions || [];
          test.questions = questions.map((q: any) => ({
            id: (q.id || q.Id)?.toString(),
            text: q.text || q.questionText || q.QuestionText || '',
            type:
              this.mapQuestionType(q.type || q.questionType || q.QuestionType) || 'SINGLE_CHOICE',
            points: q.points || q.Points || 0,
            timeLimitSeconds:
              q.timeLimitSeconds !== undefined
                ? q.timeLimitSeconds
                : q.TimeLimitSeconds !== undefined
                ? q.TimeLimitSeconds
                : null,
            options: (q.options || q.Options || []).map((o: any) => ({
              id: (o.id || o.Id)?.toString(),
              text: o.text || o.optionText || o.OptionText || '',
              isCorrect:
                o.isCorrect !== undefined
                  ? o.isCorrect
                  : o.IsCorrect !== undefined
                  ? o.IsCorrect
                  : false,
            })),
          }));
        }
        return test as Test;
      })
    );
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

  private mapQuestionType(type: any): string {
    if (!type) return 'SINGLE_CHOICE';

    const typeStr = typeof type === 'string' ? type : String(type);

    const typeMap: Record<string, string> = {
      MultipleChoice: 'MULTIPLE_CHOICE',
      MULTIPLE_CHOICE: 'MULTIPLE_CHOICE',
      '0': 'MULTIPLE_CHOICE',
      SingleChoice: 'SINGLE_CHOICE',
      SINGLE_CHOICE: 'SINGLE_CHOICE',
      TrueFalse: 'SINGLE_CHOICE',
      ShortAnswer: 'SINGLE_CHOICE',
      Essay: 'SINGLE_CHOICE',
      Coding: 'SINGLE_CHOICE',
    };

    return typeMap[typeStr] || 'SINGLE_CHOICE';
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
