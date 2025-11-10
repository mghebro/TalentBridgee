import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { CommonModule } from '@angular/common';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NzInputModule, NzButtonModule],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.scss'],
})
export class VerifyEmailComponent {
  verifyForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notification: NzNotificationService
  ) {
    this.verifyForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      code: ['', [Validators.required]]
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
        this.notification.success('Success', 'Email verified successfully!');
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err?.error?.message || 'Verification failed';
        this.notification.error('Error', msg);
      }
    });
  }
}
