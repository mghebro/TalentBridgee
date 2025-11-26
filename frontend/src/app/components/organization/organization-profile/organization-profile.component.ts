import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { OrganizationService } from '../../../services/organization/organization.service';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { OrganizationDetails } from '../../../models/organization.model';
import { Vacancy } from '../../../models/vacancy/vacancy';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-organization-profile',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    NzCardModule,
    NzAvatarModule,
    NzButtonModule,
    NzIconModule,
    NzTagModule,
    NzSpinModule,
    NzDividerModule,
    NzStatisticModule,
    NzTabsModule,
    NzEmptyModule,
    NzDescriptionsModule,
  ],
  templateUrl: './organization-profile.component.html',
  styleUrls: ['./organization-profile.component.scss'],
})
export class OrganizationProfileComponent implements OnInit {
  organization: OrganizationDetails | null = null;
  vacancies: Vacancy[] = [];
  isLoading = true;
  organizationId: string = '';

  constructor(
    private route: ActivatedRoute,
    private organizationService: OrganizationService,
    private vacancyService: VacancyService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.organizationId = this.route.snapshot.paramMap.get('id') || '';
    if (this.organizationId) {
      this.loadOrganization();
      this.loadVacancies();
    }
  }

  loadOrganization(): void {
    this.isLoading = true;
    this.organizationService.getOrganizationById(this.organizationId).subscribe({
      next: (org) => {
        this.organization = org;
        this.isLoading = false;
      },
      error: () => {
        this.notification.error('Error', 'Failed to load organization');
        this.isLoading = false;
      },
    });
  }

  loadVacancies(): void {
    this.vacancyService.getVacanciesByOrganization(parseInt(this.organizationId)).subscribe({
      next: (vacancies) => {
        this.vacancies = vacancies || [];
      },
      error: () => {
        this.vacancies = [];
      },
    });
  }

  getLogoUrl(relativePath: string | undefined | null): string {
    if (!relativePath) return '';
    const baseUrl = environment.apiUrl.replace('/api', '');
    return `${baseUrl}${relativePath}`;
  }

  getActiveVacanciesCount(): number {
    return this.vacancies.length;
  }

  getActiveVacancies(): Vacancy[] {
    return this.vacancies;
  }
}
