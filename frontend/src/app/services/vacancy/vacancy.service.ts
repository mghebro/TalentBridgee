import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  CreateVacancyRequest,
  UpdateVacancyRequest,
  Vacancy,
  VacancyDetails,
} from '../../models/vacancy/vacancy';
import { VacancyFilter } from '../../models/vacancy/vacancy-filter';
import { PaginatedResult, ServiceResult } from '../../models/api/api-response';

@Injectable({
  providedIn: 'root',
})
export class VacancyService {
  private apiUrl = `${environment.apiUrl}/vacancies`;

  constructor(private http: HttpClient) {}

  getVacancies(filter?: VacancyFilter): Observable<Vacancy[]> {
    let params = new HttpParams();
    if (filter) {
      if (filter.searchTerm) {
        params = params.append('searchTerm', filter.searchTerm);
      }
      if (filter.location) {
        params = params.append('location', filter.location);
      }
      if (filter.employmentType) {
        params = params.append('employmentType', filter.employmentType);
      }
    }
    return this.http
      .get<ServiceResult<PaginatedResult<Vacancy>>>(this.apiUrl, { params })
      .pipe(map((response) => response.data?.items ?? []));
  }

  getVacancyById(id: number | string): Observable<VacancyDetails> {
    return this.http
      .get<ServiceResult<VacancyDetails>>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => this.unwrapResult(response, 'Vacancy not found')));
  }

  createVacancy(vacancy: CreateVacancyRequest): Observable<Vacancy> {
    return this.http
      .post<ServiceResult<Vacancy>>(this.apiUrl, vacancy)
      .pipe(map((response) => this.unwrapResult(response, 'Failed to create vacancy')));
  }

  updateVacancy(id: number | string, vacancy: UpdateVacancyRequest): Observable<Vacancy> {
    return this.http
      .put<ServiceResult<Vacancy>>(`${this.apiUrl}/${id}`, vacancy)
      .pipe(map((response) => this.unwrapResult(response, 'Failed to update vacancy')));
  }

  deleteVacancy(id: number | string): Observable<void> {
    return this.http.delete<ServiceResult<string>>(`${this.apiUrl}/${id}`).pipe(map(() => void 0));
  }
  getVacanciesByOrganization(orgId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/organization/${orgId}`);
  }
  private unwrapResult<T>(response: ServiceResult<T>, fallbackMessage: string): T {
    if (!response.data) {
      throw new Error(response.message ?? response.errors?.[0] ?? fallbackMessage);
    }
    return response.data;
  }
}
