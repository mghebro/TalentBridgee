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

@Component({
  selector: 'app-vacancy-detail',
  standalone: true,
  imports: [CommonModule, NzCardModule, NzButtonModule, NzModalModule],
  templateUrl: './vacancy-detail.component.html',
  styleUrls: ['./vacancy-detail.component.scss'],
})
export class VacancyDetailComponent implements OnInit {
  vacancy: VacancyDetails | undefined;
  currentUser: User | null = null;

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
    return !!this.currentUser && this.currentUser.role === 'USER';
  }

  apply(): void {
    if (!this.currentUser) {
      this.router.navigate(['/auth/login']);
      return;
    }

    if (!this.canApply()) {
      this.notification.warning('Permission Denied', 'Only users can apply to vacancies.');
      return;
    }

    if (this.vacancy) {
      const modal = this.modalService.create({
        nzTitle: `Apply for ${this.vacancy.title}`,
        nzContent: ApplyModalComponent,
        nzData: {
          vacancyId: this.vacancy.id,
        },
        nzFooter: null,
      });

      modal.afterClose.subscribe((result) => {
        if (result) {
          // Handle successful application if needed
        }
      });
    }
  }
}
