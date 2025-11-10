import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { CommonModule } from '@angular/common';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NzInputModule, NzButtonModule, NzFormModule],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.scss'],
})
export class VerifyEmailComponent implements OnInit {
  verifyForm!: FormGroup;
  isLoading = false;
  email: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.verifyForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      code: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      if (this.email) {
        this.verifyForm.patchValue({ email: this.email });
      }
    });
  }

  submit(): void {
    if (!this.verifyForm.valid) {
      this.verifyForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    this.authService.verifyEmail(this.verifyForm.value).subscribe({
      next: () => {
        this.isLoading = false;
        this.notification.success('Success', 'Email verified successfully! You can now login.');
        this.router.navigate(['/auth/login']);
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err?.error?.message || 'Verification failed';
        this.notification.error('Error', msg);
      }
    });
  }

  resendCode(): void {
    const email = this.verifyForm.get('email')?.value;
    if (!email) {
      this.notification.error('Error', 'Please enter your email address.');
      return;
    }

    this.isLoading = true;
    this.authService.resendVerificationCode(email).subscribe({
      next: () => {
        this.isLoading = false;
        this.notification.success('Success', 'A new verification code has been sent to your email.');
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err?.error?.message || 'Failed to send verification code.';
        this.notification.error('Error', msg);
      }
    });
  }
}
