import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateVacancyRequest, UpdateVacancyRequest, VacancyDetails, VacancyList } from '../../models/vacancy/vacancy';
import { VacancyFilter } from '../../models/vacancy/vacancy-filter';
import { environment } from '../../../environments/environment';
import { PaginatedResult } from '../../models/paginated-result.model';

@Injectable({
  providedIn: 'root'
})
export class VacancyService {
  private apiUrl = environment.apiUrl + '/vacancies';

  constructor(private http: HttpClient) { }

  createVacancy(request: CreateVacancyRequest): Observable<VacancyDetails> {
    return this.http.post<VacancyDetails>(this.apiUrl, request);
  }

  getVacancies(filter: VacancyFilter): Observable<PaginatedResult<VacancyList>> {
    let params = new HttpParams();
    if (filter.search) params = params.append('search', filter.search);
    if (filter.organizationId) params = params.append('organizationId', filter.organizationId.toString());
    if (filter.profession) params = params.append('profession', filter.profession);
    if (filter.industry) params = params.append('industry', filter.industry);
    if (filter.employmentType) params = params.append('employmentType', filter.employmentType);
    if (filter.experienceLevel) params = params.append('experienceLevel', filter.experienceLevel);
    if (filter.status) params = params.append('status', filter.status);
    if (filter.location) params = params.append('location', filter.location);
    if (filter.isRemote !== undefined) params = params.append('isRemote', filter.isRemote.toString());
    if (filter.salaryMin) params = params.append('salaryMin', filter.salaryMin.toString());
    if (filter.salaryMax) params = params.append('salaryMax', filter.salaryMax.toString());
    if (filter.publishedFrom) params = params.append('publishedFrom', filter.publishedFrom.toISOString());
    if (filter.publishedTo) params = params.append('publishedTo', filter.publishedTo.toISOString());
    if (filter.page) params = params.append('page', filter.page.toString());
    if (filter.pageSize) params = params.append('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.append('sortBy', filter.sortBy);
    if (filter.sortOrder) params = params.append('sortOrder', filter.sortOrder);

    return this.http.get<PaginatedResult<VacancyList>>(this.apiUrl, { params });
  }

  getVacancyById(id: number): Observable<VacancyDetails> {
    return this.http.get<VacancyDetails>(`${this.apiUrl}/${id}`);
  }

  updateVacancy(id: number, request: UpdateVacancyRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, request);
  }

  deleteVacancy(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getVacancyAnalytics(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}/analytics`);
  }
}