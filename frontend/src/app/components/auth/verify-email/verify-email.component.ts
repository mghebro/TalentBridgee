import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { CommonModule } from '@angular/common';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzResultModule } from 'ng-zorro-antd/result';
import { extractErrorMessage } from '../../../utils/api-error';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzInputModule,
    NzButtonModule,
    NzFormModule,
    NzResultModule,
  ],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.scss'],
})
export class VerifyEmailComponent implements OnInit {
  verifyForm!: FormGroup;
  isLoading = false;
  email: string | null = null;
  invalidLink = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.verifyForm = this.fb.group({
      code: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.email = params['email'];
      if (!this.email) {
        this.invalidLink = true;
      }
    });
  }

  submit(): void {
    if (!this.verifyForm.valid || !this.email) {
      this.verifyForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    const request = {
      email: this.email,
      code: this.verifyForm.get('code')?.value,
    };

    this.authService.verifyEmail(request).subscribe({
      next: () => {
        this.isLoading = false;
        this.notification.success('Success', 'Email verified successfully! You can now login.');
        this.router.navigate(['/auth/login']);
      },
      error: (err) => {
        this.isLoading = false;
        const msg = extractErrorMessage(err, 'Verification failed');
        this.notification.error('Error', msg);
      },
    });
  }

  resendCode(): void {
    if (!this.email) {
      this.notification.error('Error', 'Email address not found.');
      return;
    }

    this.isLoading = true;
    this.authService.resendVerificationCode(this.email).subscribe({
      next: () => {
        this.isLoading = false;
        this.notification.success(
          'Success',
          'A new verification code has been sent to your email.'
        );
      },
      error: (err) => {
        this.isLoading = false;
        const msg = extractErrorMessage(err, 'Failed to send verification code.');
        this.notification.error('Error', msg);
      },
    });
  }

  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
