import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
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
    NzSelectModule
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
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
  ) { }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
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
      })
    }, { validator: this.passwordMatchValidator });

    this.registerForm.get('desiredRole')?.valueChanges.subscribe(role => {
      const orgDetails = this.registerForm.get('organizationDetails') as FormGroup;
      if (role === 'ORGANIZATION') {
        orgDetails.get('name')?.setValidators([Validators.required]);
        orgDetails.get('type')?.setValidators([Validators.required]);
        orgDetails.get('address')?.setValidators([Validators.required]);
        orgDetails.get('contactEmail')?.setValidators([Validators.required, Validators.email]);
      } else {
        orgDetails.get('name')?.clearValidators();
        orgDetails.get('type')?.clearValidators();
        orgDetails.get('address')?.clearValidators();
        orgDetails.get('contactEmail')?.clearValidators();
      }
      orgDetails.updateValueAndValidity();
    });
  }

  passwordMatchValidator(form: FormGroup) {
    return form.get('password')?.value === form.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  submitForm(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      const { confirmPassword, ...registerData } = this.registerForm.value;
      if (registerData.desiredRole !== 'ORGANIZATION') {
        delete registerData.organizationDetails;
      }

      this.authService.register(registerData).subscribe(
        () => {
          this.notification.success('Success', 'Registration successful. Please check your email to verify your account.');
          this.router.navigate(['/auth/login']);
          this.isLoading = false;
        },
        (error) => {
          this.notification.error('Error', error.message || 'Registration failed');
          this.isLoading = false;
        }
      );
    } else {
      Object.values(this.registerForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      const orgDetails = this.registerForm.get('organizationDetails') as FormGroup;
      Object.values(orgDetails.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }
}
