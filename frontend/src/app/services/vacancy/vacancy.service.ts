import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateVacancyRequest, UpdateVacancyRequest, Vacancy, VacancyDetails } from '../../models/vacancy/vacancy';
import { VacancyFilter } from '../../models/vacancy/vacancy-filter';

@Injectable({
  providedIn: 'root'
})
export class VacancyService {
  private apiUrl = `${environment.apiUrl}/vacancies`;

  constructor(private http: HttpClient) { }

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
    return this.http.get<Vacancy[]>(this.apiUrl, { params });
  }

  getVacancyById(id: string): Observable<VacancyDetails> {
    return this.http.get<VacancyDetails>(`${this.apiUrl}/${id}`);
  }

  createVacancy(vacancy: CreateVacancyRequest): Observable<Vacancy> {
    return this.http.post<Vacancy>(this.apiUrl, vacancy);
  }

  updateVacancy(id: string, vacancy: UpdateVacancyRequest): Observable<Vacancy> {
    return this.http.put<Vacancy>(`${this.apiUrl}/${id}`, vacancy);
  }

  deleteVacancy(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
