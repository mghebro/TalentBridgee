import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateOrganizationRequest, OrganizationDetails, OrganizationList, UpdateOrganizationRequest } from '../../models/organization.model';
import { OrganizationFilter } from '../../models/organization/organization-filter';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  private apiUrl = `${environment.apiUrl}/organizations`;

  constructor(private http: HttpClient) { }

  getOrganizations(filter: OrganizationFilter): Observable<OrganizationList[]> {
    let params = new HttpParams();
    if (filter.pageNumber) {
      params = params.append('pageNumber', filter.pageNumber.toString());
    }
    if (filter.pageSize) {
      params = params.append('pageSize', filter.pageSize.toString());
    }
    if (filter.name) {
      params = params.append('name', filter.name);
    }
    if (filter.type) {
      params = params.append('type', filter.type);
    }

    return this.http.get<OrganizationList[]>(this.apiUrl, { params });
  }

  getOrganizationById(id: string): Observable<OrganizationDetails> {
    return this.http.get<OrganizationDetails>(`${this.apiUrl}/${id}`);
  }

  createOrganization(request: CreateOrganizationRequest, logo?: File): Observable<OrganizationDetails> {
    const formData = new FormData();
    formData.append('name', request.name);
    formData.append('type', request.type);
    formData.append('address', request.address);
    formData.append('contactEmail', request.contactEmail);
    if (request.phoneNumber) formData.append('phoneNumber', request.phoneNumber);
    if (request.description) formData.append('description', request.description);
    if (request.website) formData.append('website', request.website);
    if (logo) {
      formData.append('logo', logo);
    }
    return this.http.post<OrganizationDetails>(this.apiUrl, formData);
  }

  updateOrganization(id: string, request: UpdateOrganizationRequest, logo?: File): Observable<OrganizationDetails> {
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

  deleteOrganization(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getMyOrganizations(): Observable<OrganizationList[]> {
    return this.http.get<OrganizationList[]>(`${this.apiUrl}/my`);
  }
}