import { Component, OnInit } from '@angular/core';
import { ApplicationService } from '../../../services/application/application.service';
import { Application } from '../../../models/application/application';
import { CommonModule } from '@angular/common';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzCardModule } from 'ng-zorro-antd/card';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-my-applications',
  standalone: true,
  imports: [CommonModule, NzListModule, NzCardModule, RouterLink],
  templateUrl: './my-applications.component.html',
  styleUrl: './my-applications.component.scss',
})
export class MyApplicationsComponent implements OnInit {
  applications: Application[] = [];

  constructor(private applicationService: ApplicationService) { }

  ngOnInit(): void {
    this.applicationService.getMyApplications().subscribe(applications => {
      this.applications = applications;
    });
  }
}
