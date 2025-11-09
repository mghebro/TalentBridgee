import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddReviewNoteRequest, Application, CreateApplicationRequest, UpdateApplicationStatusRequest } from '../../models/application/application';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private apiUrl = environment.apiUrl + '/applications';
  private vacancyApiUrl = environment.apiUrl + '/vacancies';

  constructor(private http: HttpClient) { }

  createApplication(request: CreateApplicationRequest): Observable<Application> {
    return this.http.post<Application>(this.apiUrl, request);
  }

  getApplicationsByUserId(): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.apiUrl}/my-applications`);
  }

  getApplicationsByVacancyId(vacancyId: number): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.vacancyApiUrl}/${vacancyId}/applications`);
  }

  updateApplicationStatus(id: number, request: UpdateApplicationStatusRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/status`, request);
  }

  getApplicationById(id: number): Observable<Application> {
    return this.http.get<Application>(`${this.apiUrl}/${id}`);
  }

  addReviewNote(id: number, request: AddReviewNoteRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/notes`, request);
  }
}