import { Component, OnInit } from '@angular/core';
import { ApplicationService } from '../../../services/application/application.service';
import { Application } from '../../../models/application/application';
import { CommonModule } from '@angular/common';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzCardModule } from 'ng-zorro-antd/card';
import { Router, RouterLink } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-my-applications',
  standalone: true,
  imports: [CommonModule, NzListModule, NzCardModule, RouterLink, NzButtonModule],
  templateUrl: './my-applications.component.html',
  styleUrl: './my-applications.component.scss',
})
export class MyApplicationsComponent implements OnInit {
  applications: Application[] = [];

  constructor(private applicationService: ApplicationService, private router: Router) { }

  ngOnInit(): void {
    this.applicationService.getMyApplications().subscribe(applications => {
      this.applications = applications;
    });
  }

  startTest(application: Application): void {
    this.router.navigate(['/applications', application.id, 'test']);
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
      default:
        return status;
    }
  }
}
