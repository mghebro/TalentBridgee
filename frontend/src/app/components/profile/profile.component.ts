import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzUploadModule, NzUploadChangeParam } from 'ng-zorro-antd/upload';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzTimelineModule } from 'ng-zorro-antd/timeline';
import { ProfileService } from '../../services/profile/profile.service';
import {
  Profile,
  Education,
  Experience,
  UpdateProfileRequest,
  AddEducationRequest,
  AddExperienceRequest,
} from '../../models/profile/profile';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NzCardModule,
    NzAvatarModule,
    NzButtonModule,
    NzIconModule,
    NzFormModule,
    NzInputModule,
    NzSelectModule,
    NzDividerModule,
    NzTagModule,
    NzSpinModule,
    NzModalModule,
    NzUploadModule,
    NzDatePickerModule,
    NzCheckboxModule,
    NzEmptyModule,
    NzTabsModule,
    NzTimelineModule,
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  profile: Profile | null = null;
  isLoading = true;
  isEditing = false;
  isSaving = false;

  profileForm!: FormGroup;
  educationForm!: FormGroup;
  experienceForm!: FormGroup;

  // Modals
  isEducationModalVisible = false;
  isExperienceModalVisible = false;
  editingEducationId: number | null = null;
  editingExperienceId: number | null = null;

  // File uploads
  isUploadingAvatar = false;
  isUploadingCV = false;

  genderOptions = [
    { value: 'Male', label: 'Male' },
    { value: 'Female', label: 'Female' },
    { value: 'Other', label: 'Other' },
    { value: 'PreferNotToSay', label: 'Prefer not to say' },
  ];

  constructor(
    private profileService: ProfileService,
    private fb: FormBuilder,
    private notification: NzNotificationService,
    private message: NzMessageService
  ) {
    this.initForms();
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  private initForms(): void {
    this.profileForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phoneNumber: [''],
      bio: [''],
      skills: [''],
      gender: [''],
    });

    this.educationForm = this.fb.group({
      institution: ['', Validators.required],
      degree: ['', Validators.required],
      fieldOfStudy: ['', Validators.required],
      startDate: [null, Validators.required],
      endDate: [null],
      isCurrent: [false],
      description: [''],
    });

    this.experienceForm = this.fb.group({
      company: ['', Validators.required],
      position: ['', Validators.required],
      location: [''],
      startDate: [null, Validators.required],
      endDate: [null],
      isCurrent: [false],
      description: [''],
    });
  }

  loadProfile(): void {
    this.isLoading = true;
    this.profileService.getProfile().subscribe({
      next: (response) => {
        if (response.data) {
          this.profile = response.data;
          this.populateForm();
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.notification.error('Error', 'Failed to load profile');
        this.isLoading = false;
      },
    });
  }

  private populateForm(): void {
    if (this.profile) {
      this.profileForm.patchValue({
        firstName: this.profile.firstName,
        lastName: this.profile.lastName,
        phoneNumber: this.profile.phoneNumber || '',
        bio: this.profile.bio || '',
        skills: this.profile.skills || '',
        gender: this.profile.gender || '',
      });
    }
  }

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    if (!this.isEditing) {
      this.populateForm();
    }
  }

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const request: UpdateProfileRequest = this.profileForm.value;

    this.profileService.updateProfile(request).subscribe({
      next: (response) => {
        if (response.data) {
          this.profile = response.data;
          this.notification.success('Success', 'Profile updated successfully');
          this.isEditing = false;
        }
        this.isSaving = false;
      },
      error: (err) => {
        this.notification.error('Error', 'Failed to update profile');
        this.isSaving = false;
      },
    });
  }

  // Avatar upload
  onAvatarChange(event: Event): void {
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

      this.isUploadingAvatar = true;
      this.profileService.uploadAvatar(file).subscribe({
        next: (response) => {
          if (response.data && this.profile) {
            this.profile.profilePictureUrl = response.data;
            this.notification.success('Success', 'Profile picture updated');
          }
          this.isUploadingAvatar = false;
        },
        error: (err) => {
          this.notification.error('Error', 'Failed to upload profile picture');
          this.isUploadingAvatar = false;
        },
      });
    }
  }

  deleteAvatar(): void {
    this.profileService.deleteAvatar().subscribe({
      next: () => {
        if (this.profile) {
          this.profile.profilePictureUrl = undefined;
          this.notification.success('Success', 'Profile picture removed');
        }
      },
      error: () => {
        this.notification.error('Error', 'Failed to remove profile picture');
      },
    });
  }

  // CV upload
  onCVChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];

      if (file.type !== 'application/pdf') {
        this.notification.error('Error', 'Please select a PDF file');
        return;
      }

      if (file.size > 5 * 1024 * 1024) {
        this.notification.error('Error', 'CV must be less than 5MB');
        return;
      }

      this.isUploadingCV = true;
      this.profileService.uploadCV(file).subscribe({
        next: (response) => {
          if (response.data && this.profile) {
            this.profile.cvPdfUrl = response.data;
            this.notification.success('Success', 'CV uploaded successfully');
          }
          this.isUploadingCV = false;
        },
        error: (err) => {
          this.notification.error('Error', 'Failed to upload CV');
          this.isUploadingCV = false;
        },
      });
    }
  }

  deleteCV(): void {
    this.profileService.deleteCV().subscribe({
      next: () => {
        if (this.profile) {
          this.profile.cvPdfUrl = undefined;
          this.notification.success('Success', 'CV removed');
        }
      },
      error: () => {
        this.notification.error('Error', 'Failed to remove CV');
      },
    });
  }

  getFileUrl(path: string | undefined): string | null {
    return this.profileService.getFileUrl(path);
  }

  // Education
  openAddEducationModal(): void {
    this.editingEducationId = null;
    this.educationForm.reset({ isCurrent: false });
    this.isEducationModalVisible = true;
  }

  openEditEducationModal(education: Education): void {
    this.editingEducationId = education.id;
    this.educationForm.patchValue({
      institution: education.institution,
      degree: education.degree,
      fieldOfStudy: education.fieldOfStudy,
      startDate: new Date(education.startDate),
      endDate: education.endDate ? new Date(education.endDate) : null,
      isCurrent: education.isCurrent,
      description: education.description,
    });
    this.isEducationModalVisible = true;
  }

  saveEducation(): void {
    if (this.educationForm.invalid) {
      this.educationForm.markAllAsTouched();
      return;
    }

    const request: AddEducationRequest = this.educationForm.value;

    if (this.editingEducationId) {
      this.profileService.updateEducation(this.editingEducationId, request).subscribe({
        next: () => {
          this.notification.success('Success', 'Education updated');
          this.isEducationModalVisible = false;
          this.loadProfile();
        },
        error: () => {
          this.notification.error('Error', 'Failed to update education');
        },
      });
    } else {
      this.profileService.addEducation(request).subscribe({
        next: () => {
          this.notification.success('Success', 'Education added');
          this.isEducationModalVisible = false;
          this.loadProfile();
        },
        error: () => {
          this.notification.error('Error', 'Failed to add education');
        },
      });
    }
  }

  deleteEducation(id: number): void {
    this.profileService.deleteEducation(id).subscribe({
      next: () => {
        this.notification.success('Success', 'Education removed');
        this.loadProfile();
      },
      error: () => {
        this.notification.error('Error', 'Failed to remove education');
      },
    });
  }

  // Experience
  openAddExperienceModal(): void {
    this.editingExperienceId = null;
    this.experienceForm.reset({ isCurrent: false });
    this.isExperienceModalVisible = true;
  }

  openEditExperienceModal(experience: Experience): void {
    this.editingExperienceId = experience.id;
    this.experienceForm.patchValue({
      company: experience.company,
      position: experience.position,
      location: experience.location,
      startDate: new Date(experience.startDate),
      endDate: experience.endDate ? new Date(experience.endDate) : null,
      isCurrent: experience.isCurrent,
      description: experience.description,
    });
    this.isExperienceModalVisible = true;
  }

  saveExperience(): void {
    if (this.experienceForm.invalid) {
      this.experienceForm.markAllAsTouched();
      return;
    }

    const request: AddExperienceRequest = this.experienceForm.value;

    if (this.editingExperienceId) {
      this.profileService.updateExperience(this.editingExperienceId, request).subscribe({
        next: () => {
          this.notification.success('Success', 'Experience updated');
          this.isExperienceModalVisible = false;
          this.loadProfile();
        },
        error: () => {
          this.notification.error('Error', 'Failed to update experience');
        },
      });
    } else {
      this.profileService.addExperience(request).subscribe({
        next: () => {
          this.notification.success('Success', 'Experience added');
          this.isExperienceModalVisible = false;
          this.loadProfile();
        },
        error: () => {
          this.notification.error('Error', 'Failed to add experience');
        },
      });
    }
  }

  deleteExperience(id: number): void {
    this.profileService.deleteExperience(id).subscribe({
      next: () => {
        this.notification.success('Success', 'Experience removed');
        this.loadProfile();
      },
      error: () => {
        this.notification.error('Error', 'Failed to remove experience');
      },
    });
  }

  getSkillsArray(): string[] {
    if (!this.profile?.skills) return [];
    return this.profile.skills.split(',').map((s) => s.trim()).filter((s) => s);
  }
}

