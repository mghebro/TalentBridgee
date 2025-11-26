import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { AuthService } from '../../services/auth/auth.service';
import { User } from '../../models/auth/user';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface DashboardStats {
  totalApplications?: number;
  pendingApplications?: number;
  acceptedApplications?: number;
  rejectedApplications?: number;
  testsCompleted?: number;
  testsPending?: number;
  
  totalVacancies?: number;
  activeVacancies?: number;
  totalReceived?: number;
  pendingReview?: number;
  shortlisted?: number;
  hired?: number;
  testsAssigned?: number;
  averageTestScore?: number;
}

interface RecentActivity {
  id: number;
  type: string;
  title: string;
  description: string;
  date: Date;
  status?: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    NzCardModule,
    NzGridModule,
    NzStatisticModule,
    NzIconModule,
    NzSpinModule,
    NzTagModule,
    NzTableModule,
    NzButtonModule,
    NzEmptyModule,
    NzProgressModule,
    NzAvatarModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  isLoading = true;
  stats: DashboardStats = {};
  recentActivities: RecentActivity[] = [];
  
  recentApplications: any[] = [];
  topVacancies: any[] = [];

  constructor(
    private authService: AuthService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;
    this.loadDashboardData();
  }

  private loadDashboardData(): void {
    this.isLoading = true;
    
    if (this.isUser) {
      this.loadUserDashboard();
    } else {
      this.loadOrganizationDashboard();
    }
  }

  private loadUserDashboard(): void {
    this.http.get<any>(`${environment.apiUrl}/applications/my`).subscribe({
      next: (response) => {
        const applications = response.data || [];
        this.stats = {
          totalApplications: applications.length,
          pendingApplications: applications.filter((a: any) => a.status === 'Submitted' || a.status === 'UnderReview').length,
          acceptedApplications: applications.filter((a: any) => a.status === 'Hired' || a.status === 'Shortlisted').length,
          rejectedApplications: applications.filter((a: any) => a.status === 'Rejected').length,
          testsCompleted: applications.filter((a: any) => a.testStatus === 'Completed').length,
          testsPending: applications.filter((a: any) => a.testStatus === 'Pending' || a.testStatus === 'InProgress').length,
        };
        
        this.recentActivities = applications.slice(0, 5).map((app: any) => ({
          id: app.id,
          type: 'application',
          title: app.vacancyTitle,
          description: app.organizationName,
          date: new Date(app.appliedAt),
          status: app.status,
        }));
        
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  private loadOrganizationDashboard(): void {
    Promise.all([
      this.http.get<any>(`${environment.apiUrl}/vacancies/my-vacancies`).toPromise(),
      this.http.get<any>(`${environment.apiUrl}/applications`).toPromise(),
    ]).then(([vacanciesRes, applicationsRes]) => {
      const vacancies = vacanciesRes?.data?.items || vacanciesRes?.data || [];
      const applications = applicationsRes?.data?.items || applicationsRes?.data || [];
      
      this.stats = {
        totalVacancies: vacancies.length,
        activeVacancies: vacancies.filter((v: any) => v.status === 'Active').length,
        totalReceived: applications.length,
        pendingReview: applications.filter((a: any) => a.status === 'Submitted').length,
        shortlisted: applications.filter((a: any) => a.status === 'Shortlisted').length,
        hired: applications.filter((a: any) => a.status === 'Hired').length,
      };
      
      this.recentApplications = applications.slice(0, 5);
      
      this.topVacancies = vacancies.slice(0, 5);
      
      this.isLoading = false;
    }).catch(() => {
      this.isLoading = false;
    });
  }

  get isUser(): boolean {
    return this.currentUser?.role === 'USER';
  }

  get isOrganization(): boolean {
    return this.currentUser?.role === 'ORGANIZATION_ADMIN' || this.currentUser?.role === 'HR_MANAGER';
  }

  getStatusColor(status: string): string {
    const colors: Record<string, string> = {
      'Submitted': 'blue',
      'UnderReview': 'orange',
      'Shortlisted': 'cyan',
      'Rejected': 'red',
      'Hired': 'green',
      'Active': 'green',
      'Draft': 'default',
      'Closed': 'red',
    };
    return colors[status] || 'default';
  }
}

