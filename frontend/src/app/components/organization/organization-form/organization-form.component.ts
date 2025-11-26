import { Component, Inject, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { OrganizationService } from '../../../services/organization/organization.service';
import {
  CreateOrganizationRequest,
  OrganizationDetails,
  OrganizationType,
  UpdateOrganizationRequest,
} from '../../../models/organization.model';
import { NZ_MODAL_DATA, NzModalModule, NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { CommonModule } from '@angular/common';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzUploadChangeParam, NzUploadModule } from 'ng-zorro-antd/upload';
import { environment } from '../../../../environments/environment';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { extractErrorMessage } from '../../../utils/api-error';

@Component({
  selector: 'app-organization-form',
  standalone: true,
  imports: [
    NzSpinModule,
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzSelectModule,
    NzInputModule,
    NzButtonModule,
    NzCheckboxModule,
    NzUploadModule,
    NzModalModule,
  ],
  templateUrl: './organization-form.component.html',
  styleUrls: ['./organization-form.component.scss'],
})
export class OrganizationFormComponent implements OnInit {
  @Input() organizationId?: string;
  form!: FormGroup;
  logoFile?: File;
  logoPreviewUrl?: string;
  organization?: OrganizationDetails;
  isLoading = false;
  organizationTypes = Object.values(OrganizationType);
  apiUrl = environment.apiUrl;

  constructor(
    private fb: FormBuilder,
    private organizationService: OrganizationService,
    private modalRef: NzModalRef,
    private notification: NzNotificationService,
    @Inject(NZ_MODAL_DATA) public data: { organizationId?: string }
  ) {
    this.organizationId = data?.organizationId;
  }

  ngOnInit(): void {
    this.initForm();
    if (this.organizationId) {
      this.isLoading = true;
      this.organizationService.getOrganizationById(this.organizationId!).subscribe({
        next: (org) => {
          this.organization = org;
          this.form.patchValue(org);
          this.isLoading = false;
        },
        error: (error) => {
          this.notification.error(
            'Error',
            extractErrorMessage(error, 'Failed to load organization details.')
          );
          this.isLoading = false;
        },
      });
    }
  }

  initForm(): void {
    this.form = this.fb.group({
      name: [null, [Validators.required]],
      type: [null, [Validators.required]],
      address: [null, [Validators.required]],
      contactEmail: [null, [Validators.required, Validators.email]],
      phoneNumber: [null],
      description: [null],
      website: [null],
      deleteLogo: [false],
    });
  }

  onFileChange(info: NzUploadChangeParam): void {
    if (info.file.originFileObj) {
      this.logoFile = info.file.originFileObj;
    }
  }

  onLogoFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      if (!file.type.startsWith('image/')) {
        this.notification.error('Error', 'Please select an image file');
        return;
      }

      if (file.size > 2 * 1024 * 1024) {
        this.notification.error('Error', 'Image must be less than 2MB');
        return;
      }

      this.logoFile = file;
      this.form.patchValue({ deleteLogo: false });
      
      const reader = new FileReader();
      reader.onload = (e) => {
        this.logoPreviewUrl = e.target?.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  getLogoPreviewUrl(): string | null {
    if (this.logoPreviewUrl) {
      return this.logoPreviewUrl;
    }
    if (this.organization?.logoUrl) {
      return this.organizationService.getLogoUrl(this.organization.logoUrl);
    }
    return null;
  }

  markLogoForDeletion(): void {
    this.form.patchValue({ deleteLogo: true });
    this.logoFile = undefined;
    this.logoPreviewUrl = undefined;
  }

  submitForm(): void {
    if (this.form.valid) {
      this.isLoading = true;
      const formValue = this.form.value;

      if (this.organization) {
        this.organizationService
          .updateOrganization(this.organization.id, formValue, this.logoFile)
          .subscribe({
            next: () => {
              this.notification.success('Success', 'Organization updated successfully');
              this.modalRef.close('updated');
              this.isLoading = false;
            },
            error: (error) => {
              this.notification.error(
                'Error',
                extractErrorMessage(error, 'Failed to update organization')
              );
              this.isLoading = false;
            },
          });
      } else {
        this.organizationService.createOrganization(formValue, this.logoFile).subscribe({
          next: () => {
            this.notification.success('Success', 'Organization created successfully');
            this.modalRef.close('created');
            this.isLoading = false;
          },
          error: (error) => {
            this.notification.error(
              'Error',
              extractErrorMessage(error, 'Failed to create organization')
            );
            this.isLoading = false;
          },
        });
      }
    } else {
      Object.values(this.form.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }

  cancel(): void {
    this.modalRef.destroy();
  }
}
