import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AddReviewNoteRequest, Application } from '../../models/application/application';
import { ApiResponse } from '../../models/api/api-response';
import { TestForApplicationResponse, SubmitTestRequest, TestSubmissionResponse } from '../../models/test/test';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private apiUrl = `${environment.apiUrl}/applications`;

  constructor(private http: HttpClient) { }

  createApplication(application: any): Observable<Application> {
    return this.http.post<ApiResponse<Application>>(this.apiUrl, application).pipe(
      map(response => this.unwrapResponse(response, 'Failed to submit application'))
    );
  }

  getMyApplications(): Observable<Application[]> {
    return this.http.get<ApiResponse<Application[]>>(`${this.apiUrl}/my-applications`).pipe(
      map(response => response.data ?? [])
    );
  }

  getApplicationsForVacancy(vacancyId: number): Observable<Application[]> {
    return this.http.get<ApiResponse<Application[]>>(`${environment.apiUrl}/vacancies/${vacancyId}/applications`).pipe(
      map(response => response.data ?? [])
    );
  }

  getApplicationById(id: number): Observable<Application> {
    return this.http.get<ApiResponse<Application>>(`${this.apiUrl}/${id}`).pipe(
      map(response => this.unwrapResponse(response, 'Failed to load application'))
    );
  }

  updateApplicationStatus(applicationId: number, status: string): Observable<Application> {
    return this.http.put<ApiResponse<Application>>(`${this.apiUrl}/${applicationId}/status`, { status }).pipe(
      map(response => this.unwrapResponse(response, 'Failed to update application status'))
    );
  }

  addReviewNote(applicationId: number, request: AddReviewNoteRequest): Observable<Application> {
    return this.http.post<ApiResponse<Application>>(`${this.apiUrl}/${applicationId}/notes`, request).pipe(
      map(response => this.unwrapResponse(response, 'Failed to add review note'))
    );
  }

  getTestForApplication(id: string): Observable<TestForApplicationResponse> {
    return this.http.get<TestForApplicationResponse>(`${this.apiUrl}/${id}/test`);
  }

  submitTest(id: string, request: SubmitTestRequest): Observable<TestSubmissionResponse> {
    return this.http.post<TestSubmissionResponse>(`${this.apiUrl}/${id}/submit`, request);
  }

  private unwrapResponse<T>(response: ApiResponse<T>, fallbackMessage: string): T {
    if (!response.data) {
      throw new Error(response.message ?? fallbackMessage);
    }
    return response.data;
  }
}
