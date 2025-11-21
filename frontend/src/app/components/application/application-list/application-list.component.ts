import { Component, Input, OnInit } from '@angular/core';
import { ApplicationService } from '../../../services/application/application.service';
import { Application } from '../../../models/application/application';
import { ApplicationStatus } from '../../../models/enums';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { CommonModule } from '@angular/common';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { extractErrorMessage } from '../../../utils/api-error';

@Component({
  selector: 'app-application-list',
  standalone: true,
  imports: [CommonModule, NzTableModule, NzTagModule, NzDividerModule, NzDropDownModule, RouterLink, NzIconModule, NzMenuModule],
  templateUrl: './application-list.component.html',
  styleUrls: ['./application-list.component.scss']
})
export class ApplicationListComponent implements OnInit {
  @Input() vacancyId?: string;
  applications: Application[] = [];
  loading = true;
  ApplicationStatus = ApplicationStatus; // Make enum available in template

  constructor(
    private applicationService: ApplicationService,
    private route: ActivatedRoute,
    private notification: NzNotificationService
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['vacancyId']) {
        this.vacancyId = params['vacancyId'];
        if (this.vacancyId) {
          this.loadApplicationsForVacancy(this.vacancyId);
        }
      } else {
        this.loadMyApplications();
      }
    });
  }

  loadMyApplications(): void {
    this.loading = true;
    this.applicationService.getMyApplications().subscribe({
      next: data => {
        this.applications = data;
        this.loading = false;
      },
      error: error => {
        this.notification.error('Error', extractErrorMessage(error, 'Failed to load applications'));
        this.loading = false;
      }
    });
  }

  loadApplicationsForVacancy(vacancyId: string): void {
    this.loading = true;
    const numericId = Number(vacancyId);
    if (Number.isNaN(numericId)) {
      this.notification.error('Error', 'Invalid vacancy identifier');
      this.loading = false;
      return;
    }
    this.applicationService.getApplicationsForVacancy(numericId).subscribe({
      next: data => {
        this.applications = data;
        this.loading = false;
      },
      error: error => {
        this.notification.error('Error', extractErrorMessage(error, 'Failed to load applications for vacancy'));
        this.loading = false;
      }
    });
  }

  updateStatus(application: Application, status: string): void {
    this.applicationService.updateApplicationStatus(application.id, status as ApplicationStatus).subscribe({
      next: () => {
        this.notification.success('Success', 'Application status updated successfully');
        application.status = status;
      },
      error: error => {
        this.notification.error('Error', extractErrorMessage(error, 'Failed to update application status'));
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case ApplicationStatus.Hired:
        return 'green';
      case ApplicationStatus.Offered:
        return 'blue';
      case ApplicationStatus.Interview:
        return 'orange';
      case ApplicationStatus.Shortlisted:
        return 'gold';
      case ApplicationStatus.UnderReview:
        return 'geekblue';
      case ApplicationStatus.Submitted:
        return 'default';
      case ApplicationStatus.Rejected:
        return 'red';
      case ApplicationStatus.Withdrawn:
        return 'volcano';
      case ApplicationStatus.TestAssigned:
        return 'purple';
      default:
        return 'default';
    }
  }

  getApplicationStatusValues(): string[] {
    return Object.values(ApplicationStatus);
  }
}
