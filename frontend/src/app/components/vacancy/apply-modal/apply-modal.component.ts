import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ApplicationService } from '../../../services/application/application.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzIconModule } from 'ng-zorro-antd/icon';

@Component({
  selector: 'app-apply-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzUploadModule,
    NzIconModule
  ],
  templateUrl: './apply-modal.component.html',
  styleUrls: ['./apply-modal.component.scss']
})
export class ApplyModalComponent implements OnInit {
  @Input() vacancyId!: string;
  applyForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private applicationService: ApplicationService,
    private notification: NzNotificationService,
    private modalRef: NzModalRef
  ) { }

  ngOnInit(): void {
    this.applyForm = this.fb.group({
      resume: [null, [Validators.required]]
    });
  }

  beforeUpload = (file: any): boolean => {
    this.applyForm.patchValue({ resume: file });
    return false;
  };

  handleUpload(event: any): void {
    // The file is already handled by beforeUpload
  }

  submitForm(): void {
    if (this.applyForm.valid) {
      this.isLoading = true;
      const formData = new FormData();
      formData.append('vacancyId', this.vacancyId);
      formData.append('resume', this.applyForm.value.resume);

      this.applicationService.createApplication(formData).subscribe({
        next: () => {
          this.notification.success('Success', 'Application submitted successfully');
          this.isLoading = false;
          this.modalRef.close(true);
        },
        error: (error) => {
          this.notification.error('Error', error.error.message || 'Failed to submit application');
          this.isLoading = false;
        }
      });
    }
  }

  closeModal(): void {
    this.modalRef.close();
  }
}
