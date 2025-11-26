import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../models/api/api-response';
import {
  Profile,
  UpdateProfileRequest,
  Education,
  Experience,
  AddEducationRequest,
  AddExperienceRequest,
} from '../../models/profile/profile';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private apiUrl = `${environment.apiUrl}/profile`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<ApiResponse<Profile>> {
    return this.http.get<ApiResponse<Profile>>(`${this.apiUrl}/user`);
  }

  getProfileByUserId(userId: number): Observable<ApiResponse<Profile>> {
    return this.http.get<ApiResponse<Profile>>(`${this.apiUrl}/user/${userId}`);
  }

  updateProfile(request: UpdateProfileRequest): Observable<ApiResponse<Profile>> {
    return this.http.put<ApiResponse<Profile>>(`${this.apiUrl}/update-user`, request);
  }

  uploadCV(file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/upload-cv`, formData);
  }

  uploadAvatar(file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/upload-avatar`, formData);
  }

  deleteCV(): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/cv`);
  }

  deleteAvatar(): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/avatar`);
  }

  addEducation(request: AddEducationRequest): Observable<ApiResponse<Education>> {
    return this.http.post<ApiResponse<Education>>(`${this.apiUrl}/education`, request);
  }

  updateEducation(
    educationId: number,
    request: AddEducationRequest
  ): Observable<ApiResponse<Education>> {
    return this.http.put<ApiResponse<Education>>(
      `${this.apiUrl}/education/${educationId}`,
      request
    );
  }

  deleteEducation(educationId: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/education/${educationId}`);
  }

  addExperience(request: AddExperienceRequest): Observable<ApiResponse<Experience>> {
    return this.http.post<ApiResponse<Experience>>(`${this.apiUrl}/experience`, request);
  }

  updateExperience(
    experienceId: number,
    request: AddExperienceRequest
  ): Observable<ApiResponse<Experience>> {
    return this.http.put<ApiResponse<Experience>>(
      `${this.apiUrl}/experience/${experienceId}`,
      request
    );
  }

  deleteExperience(experienceId: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/experience/${experienceId}`);
  }
  
  getFileUrl(relativePath: string | undefined): string | null {
    if (!relativePath) return null;
    const baseUrl = environment.apiUrl.replace('/api', '');
    return `${baseUrl}${relativePath}`;
  }
}
