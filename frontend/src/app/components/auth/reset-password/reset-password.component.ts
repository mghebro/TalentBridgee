import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzResultModule } from 'ng-zorro-antd/result';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { extractErrorMessage } from '../../../utils/api-error';
import { PASSWORD_PATTERN } from '../../../utils/validation-patterns';

@Component({
  selector: 'app-reset-password',
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
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit {
  form: FormGroup;
  token: string | null = null;
  isLoading = false;
  resetCompleted = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private notification: NzNotificationService
  ) {
    this.form = this.fb.group({
      newPassword: [
        '',
        [Validators.required, Validators.minLength(8), Validators.pattern(PASSWORD_PATTERN)],
      ],
      confirmPassword: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'] ?? null;
    });
  }

  submit(): void {
    if (!this.token) {
      this.notification.error('Invalid link', 'Reset link is missing or expired.');
      return;
    }

    if (this.form.invalid || this.passwordsDoNotMatch) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const payload = {
      token: this.token,
      newPassword: this.form.value.newPassword,
      confirmPassword: this.form.value.confirmPassword,
    };

    this.authService.resetPassword(payload).subscribe({
      next: () => {
        this.isLoading = false;
        this.resetCompleted = true;
        this.notification.success('Password updated', 'You can now login with your new password.');
      },
      error: (error) => {
        this.isLoading = false;
        this.notification.error('Error', extractErrorMessage(error, 'Unable to reset password'));
      },
    });
  }

  get passwordsDoNotMatch(): boolean {
    const { newPassword, confirmPassword } = this.form.value;
    return newPassword && confirmPassword && newPassword !== confirmPassword;
  }

  get newPasswordControl() {
    return this.form.get('newPassword');
  }

  get confirmPasswordControl() {
    return this.form.get('confirmPassword');
  }

  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
