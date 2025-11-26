import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { OrganizationType } from '../../../models/organization.model';
import { extractErrorMessage } from '../../../utils/api-error';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    RouterLink,
    NzSelectModule,
    NzIconModule,
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  isLoading = false;
  organizationTypes = Object.values(OrganizationType);
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group(
      {
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required]],
        firstName: ['', [Validators.required]],
        lastName: ['', [Validators.required]],
        desiredRole: ['USER', [Validators.required]],
        organizationDetails: this.fb.group({
          name: [''],
          type: [''],
          address: [''],
          contactEmail: [''],
          website: [''],
          description: [''],
          phoneNumber: [''],
        }),
      },
      { validators: this.passwordMatchValidator }
    );

    this.registerForm.get('desiredRole')?.valueChanges.subscribe((role) => {
      this.updateOrganizationValidators(role);
    });
  }

  private updateOrganizationValidators(role: string): void {
    const orgDetails = this.registerForm.get('organizationDetails') as FormGroup;
    const fields = [
      'name',
      'type',
      'address',
      'contactEmail',
      'website',
      'description',
      'phoneNumber',
    ];

    if (role === 'ORGANIZATION_ADMIN') {
      orgDetails.get('name')?.setValidators([Validators.required]);
      orgDetails.get('type')?.setValidators([Validators.required]);
      orgDetails.get('address')?.setValidators([Validators.required]);
      orgDetails.get('contactEmail')?.setValidators([Validators.required, Validators.email]);
      orgDetails.get('website')?.setValidators([Validators.required]);
      orgDetails.get('description')?.setValidators([Validators.required]);
      orgDetails.get('phoneNumber')?.setValidators([Validators.required]);
    } else {
      fields.forEach((field) => {
        orgDetails.get(field)?.clearValidators();
        orgDetails.get(field)?.reset();
      });
    }

    fields.forEach((field) => {
      orgDetails.get(field)?.updateValueAndValidity();
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { mismatch: true };
  }

  submitForm(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      const { confirmPassword, ...registerData } = this.registerForm.value;

      if (registerData.desiredRole !== 'ORGANIZATION_ADMIN') {
        delete registerData.organizationDetails;
      }

      this.authService.register(registerData).subscribe({
        next: (response) => {
          this.notification.success(
            'Success',
            'Registration successful. Please check your email to verify your account.'
          );
          this.router.navigate(['/auth/verify-email'], {
            queryParams: { email: registerData.email },
          });
          this.isLoading = false;
        },
        error: (error) => {
          const errorMessage = extractErrorMessage(error, 'Registration failed');
          this.notification.error('Error', errorMessage);
          this.isLoading = false;
        },
      });
    } else {
      this.markFormGroupTouched(this.registerForm);
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      control?.markAsDirty();
      control?.updateValueAndValidity({ onlySelf: true });

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
}
