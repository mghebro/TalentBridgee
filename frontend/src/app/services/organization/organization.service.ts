import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { CreateOrganizationRequest, OrganizationDetails, OrganizationList, UpdateOrganizationRequest } from '../../models/organization.model';
import { OrganizationFilter } from '../../models/organization/organization-filter';
import { PaginatedResult, ServiceResult } from '../../models/api/api-response';

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

    return this.http.get<ServiceResult<PaginatedResult<OrganizationList>>>(this.apiUrl, { params }).pipe(
      map(response => response.data?.items ?? [])
    );
  }

  getOrganizationById(id: string): Observable<OrganizationDetails> {
    return this.http.get<ServiceResult<OrganizationDetails>>(`${this.apiUrl}/${id}`).pipe(
      map(response => this.unwrapResult(response, 'Organization not found'))
    );
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
    return this.http.post<ServiceResult<OrganizationDetails>>(this.apiUrl, formData).pipe(
      map(response => this.unwrapResult(response, 'Failed to create organization'))
    );
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
    return this.http.put<ServiceResult<OrganizationDetails>>(`${this.apiUrl}/${id}`, formData).pipe(
      map(response => this.unwrapResult(response, 'Failed to update organization'))
    );
  }

  deleteOrganization(id: string): Observable<void> {
    return this.http.delete<ServiceResult<string>>(`${this.apiUrl}/${id}`).pipe(map(() => void 0));
  }

  getMyOrganizations(): Observable<OrganizationList[]> {
    return this.http.get<ServiceResult<OrganizationList[]>>(`${this.apiUrl}/my`).pipe(
      map(response => response.data ?? [])
    );
  }

  uploadLogo(id: string, file: File): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ServiceResult<string>>(`${this.apiUrl}/${id}/logo`, formData).pipe(
      map(response => this.unwrapResult(response, 'Failed to upload logo'))
    );
  }

  deleteLogo(id: string): Observable<boolean> {
    return this.http.delete<ServiceResult<boolean>>(`${this.apiUrl}/${id}/logo`).pipe(
      map(response => response.data ?? false)
    );
  }

  getLogoUrl(relativePath: string | undefined | null): string | null {
    if (!relativePath) return null;
    const baseUrl = environment.apiUrl.replace('/api', '');
    return `${baseUrl}${relativePath}`;
  }

  private unwrapResult<T>(response: ServiceResult<T>, fallbackMessage: string): T {
    if (!response.data) {
      throw new Error(response.message ?? response.errors?.[0] ?? fallbackMessage);
    }
    return response.data;
  }
}