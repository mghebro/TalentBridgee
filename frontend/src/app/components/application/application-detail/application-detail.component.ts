import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApplicationService } from '../../../services/application/application.service'; // Corrected import path
import { AddReviewNoteRequest, Application, ApplicationStatus } from '../../../models/application/application';
import { NzSpinComponent } from 'ng-zorro-antd/spin';
import { NzPageHeaderComponent, NzPageHeaderModule, NzPageHeaderSubtitleDirective } from 'ng-zorro-antd/page-header';
import { CommonModule } from '@angular/common';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzFormModule } from 'ng-zorro-antd/form'; // Added NzFormModule

@Component({
  selector: 'app-application-detail',
  standalone: true,
  imports: [
    NzTagModule,
    NzSpinComponent,
    NzPageHeaderModule,
    CommonModule,
    NzDescriptionsModule,
    ReactiveFormsModule,
    NzInputModule,
    NzButtonModule,
    NzFormModule // Added NzFormModule
  ],
  templateUrl: './application-detail.component.html',
  styleUrls: ['./application-detail.component.scss']
})
export class ApplicationDetailComponent implements OnInit {
  application?: Application;
  loading = true;
  reviewNoteForm!: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private applicationService: ApplicationService,
    private fb: FormBuilder,
    private notification: NzNotificationService
  ) { }

  ngOnInit(): void {
    this.reviewNoteForm = this.fb.group({
      note: ['', Validators.required]
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.applicationService.getApplicationById(+id).subscribe(
        data => {
          this.application = data;
          this.loading = false;
        },
        error => {
          this.notification.error('Error', error.error.message || 'Failed to load application details');
          this.loading = false;
        }
      );
    }
  }

  addReviewNote(): void {
    if (this.reviewNoteForm.valid && this.application?.id) {
      const request: AddReviewNoteRequest = this.reviewNoteForm.value;
      this.applicationService.addReviewNote(this.application.id, request).subscribe(
        () => {
          this.notification.success('Success', 'Review note added successfully');
          this.reviewNoteForm.reset();
          // Optionally refresh application details to show the new note
          // For now, assuming notes are not part of the Application model directly
        },
        error => {
          this.notification.error('Error', error.error.message || 'Failed to add review note');
        }
      );
    }
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
}
