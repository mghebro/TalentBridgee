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
import { OrganizationType } from '../../../models/organization.model';

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
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  isLoading = false;
  organizationTypes = Object.values(OrganizationType);

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

    // Update validity for each control individually
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

      // Remove organizationDetails if not ORGANIZATION_ADMIN
      if (registerData.desiredRole !== 'ORGANIZATION_ADMIN') {
        delete registerData.organizationDetails;
      }

      // Send data directly, not wrapped in { request: ... }
      this.authService.register(registerData).subscribe({
        next: (response) => {
          this.notification.success(
            'Success',
            'Registration successful. Please check your email to verify your account.'
          );
         this.router.navigate(['/auth/verify-email']);

          this.isLoading = false;
        },
        error: (error) => {
          const errorMessage = error?.error?.message || error?.message || 'Registration failed';
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
}
