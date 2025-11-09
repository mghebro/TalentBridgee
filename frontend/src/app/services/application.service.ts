import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  Application,
  CreateApplicationRequest,
  UpdateApplicationStatusRequest,
  AddReviewNoteRequest
} from '../models/application.model';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private apiUrl = `${environment.apiUrl}/applications`;

  constructor(private http: HttpClient) { }

  createApplication(request: CreateApplicationRequest): Observable<Application> {
    return this.http.post<Application>(this.apiUrl, request);
  }

  getApplicationsByUserId(): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.apiUrl}/my-applications`);
  }

  getApplicationsByVacancyId(vacancyId: number): Observable<Application[]> {
    return this.http.get<Application[]>(`${environment.apiUrl}/vacancies/${vacancyId}/applications`);
  }

  updateApplicationStatus(id: number, status: string): Observable<Application> {
    const request: UpdateApplicationStatusRequest = { status: status as any }; // TODO: Fix type casting
    return this.http.put<Application>(`${this.apiUrl}/${id}/status`, request);
  }

  getApplicationById(id: number): Observable<Application> {
    return this.http.get<Application>(`${this.apiUrl}/${id}`);
  }

  addReviewNote(id: number, request: AddReviewNoteRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/notes`, request);
  }
}
