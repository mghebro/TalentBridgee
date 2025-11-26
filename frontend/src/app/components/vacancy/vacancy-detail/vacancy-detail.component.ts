import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { VacancyDetails } from '../../../models/vacancy/vacancy';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { AuthService } from '../../../services/auth/auth.service';
import { User } from '../../../models/auth/user';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ApplyModalComponent } from '../apply-modal/apply-modal.component';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-vacancy-detail',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzButtonModule,
    NzModalModule,
    NzAvatarModule,
    NzTagModule,
    NzIconModule,
    NzSpinModule,
  ],
  templateUrl: './vacancy-detail.component.html',
  styleUrls: ['./vacancy-detail.component.scss'],
})
export class VacancyDetailComponent implements OnInit {
  vacancy: VacancyDetails | undefined;
  currentUser: User | null = null;
  hasApplied = false;
  isApplying = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private vacancyService: VacancyService,
    private authService: AuthService,
    private modalService: NzModalService,
    private notification: NzNotificationService
  ) {
    this.authService.currentUser.subscribe((user) => {
      this.currentUser = user;
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.vacancyService.getVacancyById(id).subscribe((vacancy) => {
        this.vacancy = vacancy;
      });
    }
  }

  canViewApplications(): boolean {
    return (
      !!this.currentUser && ['ORGANIZATION_ADMIN', 'HR_MANAGER'].includes(this.currentUser.role)
    );
  }

  viewApplications(): void {
    if (!this.vacancy) {
      return;
    }

    this.router.navigate(['/vacancies', this.vacancy.id, 'applications']);
  }

  canApply(): boolean {
    return (
      !!this.currentUser && this.currentUser.role === 'USER' && !this.hasApplied && !this.isApplying
    );
  }

  apply(): void {
    if (!this.currentUser) {
      this.router.navigate(['/auth/login']);
      return;
    }

    if (!this.canApply()) {
      return;
    }

    this.isApplying = true;

    if (this.vacancy) {
      this.vacancyService.applyToVacancy(this.vacancy.id).subscribe({
        next: () => {
          this.notification.success('Success', 'You have successfully applied for this vacancy.');
          this.hasApplied = true;
          this.isApplying = false;
        },
        error: (err) => {
          this.notification.error('Error', err.message);
          this.isApplying = false;
        },
      });
    } else {
      this.isApplying = false;
    }
  }

  getLogoUrl(logoUrl: string | null | undefined): string | undefined {
    if (!logoUrl) return undefined;
    const baseUrl = environment.apiUrl.replace('/api', '');
    return `${baseUrl}${logoUrl}`;
  }

  formatContent(content: string | null | undefined): string {
    if (!content) return '';
    return content.replace(/\n/g, '<br>');
  }
}
