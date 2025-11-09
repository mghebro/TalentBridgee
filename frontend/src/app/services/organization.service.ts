import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  OrganizationList,
  CreateOrganizationRequest,
  UpdateOrganizationRequest,
  OrganizationFilterRequest,
  OrganizationDetails
} from '../models/organization.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class OrganizationService {
  private apiUrl = `${environment.apiUrl}/organizations`;

  constructor(private http: HttpClient) {}

  createOrganization(request: CreateOrganizationRequest, logo?: File): Observable<OrganizationDetails> {
    const formData = new FormData();
    formData.append('name', request.name);
    formData.append('type', request.type.toString());
    formData.append('address', request.address);
    formData.append('contactEmail', request.contactEmail);
    formData.append('phoneNumber', request.phoneNumber ?? '');
    formData.append('description', request.description ?? '');
    formData.append('website', request.website ?? '');

    if (logo) {
      formData.append('logo', logo);
    }
    return this.http.post<OrganizationDetails>(this.apiUrl, formData);
  }

  getOrganizations(filter: OrganizationFilterRequest): Observable<OrganizationList[]> {
    let params = new HttpParams();
    if (filter.search) params = params.append('search', filter.search);
    if (filter.type) params = params.append('type', filter.type.toString());
    if (filter.location) params = params.append('location', filter.location);
    if (filter.isActive !== undefined)
      params = params.append('isActive', filter.isActive.toString());
    if (filter.page) params = params.append('page', filter.page.toString());
    if (filter.pageSize) params = params.append('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.append('sortBy', filter.sortBy);
    if (filter.sortOrder) params = params.append('sortOrder', filter.sortOrder);

    return this.http.get<OrganizationList[]>(this.apiUrl, { params });
  }

  getOrganizationById(id: number): Observable<OrganizationDetails> {
    return this.http.get<OrganizationDetails>(`${this.apiUrl}/${id}`);
  }

  updateOrganization(
    id: number,
    request: UpdateOrganizationRequest,
    logo?: File
  ): Observable<OrganizationDetails> {
    const formData = new FormData();
    if (request.name) formData.append('name', request.name);
    if (request.address) formData.append('address', request.address);
    if (request.contactEmail) formData.append('contactEmail', request.contactEmail);
    if (request.phoneNumber) formData.append('phoneNumber', request.phoneNumber);
    if (request.description) formData.append('description', request.description);
    if (request.website) formData.append('website', request.website);
    if (request.deleteLogo) formData.append('deleteLogo', request.deleteLogo.toString());
    if (logo) {
      formData.append('logo', logo);
    }
    return this.http.put<OrganizationDetails>(`${this.apiUrl}/${id}`, formData);
  }

  deleteOrganization(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getOrganizationsForCurrentUser(): Observable<OrganizationList[]> {
    return this.http.get<OrganizationList[]>(`${this.apiUrl}/my`);
  }
}
