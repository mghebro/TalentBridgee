import { Component, OnInit } from '@angular/core';
import { ApplicationService } from '../../../services/application/application.service';
import { Application } from '../../../models/application/application';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { Router, RouterLink } from '@angular/router';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzSpinModule } from 'ng-zorro-antd/spin';

@Component({
  selector: 'app-my-applications',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    RouterLink,
    NzTagModule,
    NzIconModule,
    NzEmptyModule,
    NzSpinModule,
  ],
  templateUrl: './my-applications.component.html',
  styleUrl: './my-applications.component.scss',
})
export class MyApplicationsComponent implements OnInit {
  applications: Application[] = [];
  isLoading = true;

  constructor(private applicationService: ApplicationService, private router: Router) {}

  ngOnInit(): void {
    this.isLoading = true;
    this.applicationService.getMyApplications().subscribe({
      next: (applications) => {
        this.applications = applications;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'TestAssigned':
        return 'Test Assigned';
      case 'TestInProgress':
        return 'Test In Progress';
      case 'TestPassed':
        return 'Test Passed';
      case 'TestFailed':
        return 'Test Failed';
      case 'Submitted':
        return 'Submitted';
      case 'UnderReview':
        return 'Under Review';
      case 'Shortlisted':
        return 'Shortlisted';
      case 'Interview':
        return 'Interview';
      case 'Offered':
        return 'Offered';
      case 'Hired':
        return 'Hired';
      case 'Rejected':
        return 'Rejected';
      case 'Withdrawn':
        return 'Withdrawn';
      default:
        return status;
    }
  }

  getTestStatusText(status?: string): string {
    if (!status) return 'N/A';
    switch (status) {
      case 'Assigned':
        return 'Assigned';
      case 'InProgress':
        return 'In Progress';
      case 'Completed':
        return 'Completed';
      case 'Expired':
        return 'Expired';
      default:
        return status;
    }
  }

  getTestStatusColor(status?: string): string {
    if (!status) return 'default';
    switch (status) {
      case 'Assigned':
        return 'blue';
      case 'InProgress':
        return 'orange';
      case 'Completed':
        return 'green';
      case 'Expired':
        return 'red';
      default:
        return 'default';
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Hired':
        return 'green';
      case 'Rejected':
        return 'red';
      case 'Shortlisted':
      case 'Interview':
      case 'Offered':
        return 'blue';
      case 'TestPassed':
        return 'green';
      case 'TestFailed':
        return 'red';
      case 'TestAssigned':
      case 'TestInProgress':
        return 'orange';
      case 'UnderReview':
        return 'cyan';
      default:
        return 'default';
    }
  }
}
