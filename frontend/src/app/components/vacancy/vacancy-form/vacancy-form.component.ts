import { Component, Inject, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import {
  CreateVacancyRequest,
  UpdateVacancyRequest,
  VacancyDetails,
} from '../../../models/vacancy/vacancy';
import { NZ_MODAL_DATA, NzModalModule, NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { OrganizationService } from '../../../services/organization/organization.service';
import { OrganizationList } from '../../../models/organization.model';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { CommonModule } from '@angular/common';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { EmploymentType, ExperienceLevel, VacancyStatus } from '../../../models/enums';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { extractErrorMessage } from '../../../utils/api-error';

@Component({
  selector: 'app-vacancy-form',
  standalone: true,
  imports: [
    NzInputNumberModule,
    NzSpinModule,
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzSelectModule,
    NzInputModule,
    NzDatePickerModule,
    NzButtonModule,
    NzCheckboxModule,
    NzInputNumberModule,
    NzModalModule,
  ],
  templateUrl: './vacancy-form.component.html',
  styleUrls: ['./vacancy-form.component.scss'],
})
export class VacancyFormComponent implements OnInit {
  @Input() vacancyId?: number | string;
  form!: FormGroup;
  organizations: OrganizationList[] = [];
  vacancy?: VacancyDetails;
  isLoading = false;

  // Enums for template
  EmploymentType = Object.values(EmploymentType);
  ExperienceLevel = Object.values(ExperienceLevel);
  VacancyStatus = Object.values(VacancyStatus);

  constructor(
    private fb: FormBuilder,
    private vacancyService: VacancyService,
    private organizationService: OrganizationService,
    private modalRef: NzModalRef,
    private notification: NzNotificationService,
    @Inject(NZ_MODAL_DATA) public data: { vacancyId?: number | string }
  ) {
    this.vacancyId = data?.vacancyId;
  }

  ngOnInit(): void {
    this.initForm();
    this.loadOrganizations();
    if (this.vacancyId) {
      this.isLoading = true;
      this.vacancyService.getVacancyById(this.vacancyId!).subscribe({
        next: (vacancy) => {
          this.vacancy = vacancy;
          // Properly map the vacancy data to form
          this.form.patchValue({
            title: vacancy.title,
            organizationId: vacancy.organizationId,
            description: vacancy.description,
            requirements: vacancy.requirements,
            responsibilities: vacancy.responsibilities,
            profession: vacancy.profession,
            industry: vacancy.industry,
            employmentType: vacancy.employmentType,
            experienceLevel: vacancy.experienceLevel,
            salaryMin: vacancy.salaryMin,
            salaryMax: vacancy.salaryMax,
            salaryCurrency: vacancy.salaryCurrency || 'GEL',
            location: vacancy.location,
            isRemote: vacancy.isRemote,
            status: vacancy.status,
            applicationDeadline: vacancy.applicationDeadline ? new Date(vacancy.applicationDeadline) : null,
          });
          this.isLoading = false;
        },
        error: (error) => {
          this.notification.error(
            'Error',
            extractErrorMessage(error, 'Failed to load vacancy details.')
          );
          this.isLoading = false;
        },
      });
    }
  }

  initForm(): void {
    this.form = this.fb.group({
      title: [null, [Validators.required]],
      organizationId: [null, [Validators.required]],
      description: [null, [Validators.required]],
      requirements: [null, [Validators.required]],
      responsibilities: [null, [Validators.required]],
      profession: [null, [Validators.required]],
      industry: [null, [Validators.required]],
      employmentType: [null, [Validators.required]],
      experienceLevel: [null, [Validators.required]],
      salaryMin: [null],
      salaryMax: [null],
      salaryCurrency: ['GEL'],
      location: [null, [Validators.required]],
      isRemote: [false],
      status: [VacancyStatus.Draft, [Validators.required]],
      applicationDeadline: [null, [Validators.required]],
    });
  }

  loadOrganizations(): void {
    this.organizationService.getMyOrganizations().subscribe((orgs) => {
      this.organizations = orgs;
    });
  }

  submitForm(): void {
    if (this.form.valid) {
      this.isLoading = true;
      const formValue = {
        ...this.form.value,
        organizationId: Number(this.form.value.organizationId),
      };

      if (Number.isNaN(formValue.organizationId)) {
        this.notification.error('Error', 'Invalid organization selected');
        this.isLoading = false;
        return;
      }
      if (this.vacancy) {
        const updateRequest: UpdateVacancyRequest = formValue;
        this.vacancyService.updateVacancy(this.vacancy.id, updateRequest).subscribe({
          next: () => {
            this.notification.success('Success', 'Vacancy updated successfully');
            this.modalRef.close('updated');
            this.isLoading = false;
          },
          error: (error) => {
            this.notification.error(
              'Error',
              extractErrorMessage(error, 'Failed to update vacancy')
            );
            this.isLoading = false;
          },
        });
      } else {
        const createRequest: CreateVacancyRequest = formValue;
        this.vacancyService.createVacancy(createRequest).subscribe({
          next: () => {
            this.notification.success('Success', 'Vacancy created successfully');
            this.modalRef.close('created');
          },
          error: (error) => {
            this.notification.error(
              'Error',
              extractErrorMessage(error, 'Failed to create vacancy')
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
