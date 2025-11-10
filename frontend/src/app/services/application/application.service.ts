import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AddReviewNoteRequest, Application } from '../../models/application/application';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private apiUrl = `${environment.apiUrl}/applications`;

  constructor(private http: HttpClient) { }

  createApplication(application: any): Observable<Application> {
    return this.http.post<Application>(this.apiUrl, application);
  }

  getMyApplications(): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.apiUrl}/my-applications`);
  }

  getApplicationsForVacancy(vacancyId: string): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.apiUrl}/vacancy/${vacancyId}`);
  }

  getApplicationById(id: string): Observable<Application> {
    return this.http.get<Application>(`${this.apiUrl}/${id}`);
  }

  updateApplicationStatus(applicationId: string, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${applicationId}/status`, { status });
  }

  addReviewNote(applicationId: string, request: AddReviewNoteRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/${applicationId}/notes`, request);
  }
}
