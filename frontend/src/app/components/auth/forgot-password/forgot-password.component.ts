import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzResultModule } from 'ng-zorro-antd/result';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { extractErrorMessage } from '../../../utils/api-error';
import { EMAIL_PATTERN } from '../../../utils/validation-patterns';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzResultModule,
    RouterLink,
    NzIconModule,
  ],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent {
  form: FormGroup;
  isLoading = false;
  linkSentTo: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notification: NzNotificationService
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.pattern(EMAIL_PATTERN)]],
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const email = this.form.value.email;

    this.authService.sendResetPasswordLink(email).subscribe({
      next: () => {
        this.isLoading = false;
        this.linkSentTo = email;
        this.notification.success('Email sent', 'Check your inbox for the reset link.');
      },
      error: (error) => {
        this.isLoading = false;
        this.linkSentTo = null;
        this.notification.error('Error', extractErrorMessage(error, 'Unable to send email'));
      },
    });
  }

  resend(): void {
    if (!this.linkSentTo) {
      return;
    }
    this.isLoading = true;
    this.authService.sendResetPasswordLink(this.linkSentTo).subscribe({
      next: () => {
        this.isLoading = false;
        this.notification.success('Email sent', 'We sent you another reset link.');
      },
      error: (error) => {
        this.isLoading = false;
        this.notification.error('Error', extractErrorMessage(error, 'Unable to resend email'));
      },
    });
  }

  get emailControl() {
    return this.form.get('email');
  }
}
