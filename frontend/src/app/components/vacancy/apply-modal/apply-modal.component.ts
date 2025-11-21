import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ApplicationService } from '../../../services/application/application.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalModule, NzModalRef } from 'ng-zorro-antd/modal';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { extractErrorMessage } from '../../../utils/api-error';

@Component({
  selector: 'app-apply-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzIconModule,
    NzModalModule,
  ],
  templateUrl: './apply-modal.component.html',
  styleUrls: ['./apply-modal.component.scss'],
})
export class ApplyModalComponent implements OnInit {
  @Input() vacancyId!: number;
  applyForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private applicationService: ApplicationService,
    private notification: NzNotificationService,
    private modalRef: NzModalRef
  ) {}

  ngOnInit(): void {
    this.applyForm = this.fb.group({
      coverLetter: [''],
    });
  }

  submitForm(): void {
    if (this.applyForm.valid) {
      this.isLoading = true;
      const payload = {
        vacancyId: Number(this.vacancyId),
        coverLetter: this.applyForm.value.coverLetter,
      };

      this.applicationService.createApplication(payload).subscribe({
        next: () => {
          this.notification.success('Success', 'Application submitted successfully');
          this.isLoading = false;
          this.modalRef.close(true);
        },
        error: (error) => {
          this.notification.error(
            'Error',
            extractErrorMessage(error, 'Failed to submit application')
          );
          this.isLoading = false;
        },
      });
    }
  }

  closeModal(): void {
    this.modalRef.close();
  }
}
