import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { Vacancy } from '../../../models/vacancy/vacancy';
import { CommonModule } from '@angular/common';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { VacancyFormComponent } from '../vacancy-form/vacancy-form.component';
import { OrganizationService } from '../../../services/organization/organization.service';
import { Organization } from '../../../models/organization/organization';
import { OrganizationList } from '../../../models/organization.model';
import { AssignTestModalComponent } from '../assign-test-modal/assign-test-modal.component';

@Component({
  selector: 'app-vacancy-management',
  standalone: true,
  imports: [CommonModule, NzTableModule, NzButtonModule, NzModalModule, RouterLink],
  templateUrl: './vacancy-management.component.html',
  styleUrls: ['./vacancy-management.component.scss'],
})
export class VacancyManagementComponent implements OnInit {
  vacancies: Vacancy[] = [];
  myOrganizations: OrganizationList[] = [];

  constructor(
    private vacancyService: VacancyService,
    private organizationService: OrganizationService,
    private modalService: NzModalService
  ) {}

  ngOnInit(): void {
    this.organizationService.getMyOrganizations().subscribe((orgs) => {
      this.myOrganizations = orgs;
      if (orgs.length > 0) {
        orgs.forEach((element) => {
          this.loadVacancies(element.id);
        });
      }
    });
  }

  loadVacancies(organizationId: number): void {
    const orgId = Number(organizationId);
    this.vacancyService.getVacancies().subscribe((vacancies) => {
      this.vacancies = Number.isNaN(orgId)
        ? vacancies
        : vacancies.filter((v) => v.organizationId === orgId);
    });
  }

  openVacancyModal(vacancyId?: number): void {
    const modal = this.modalService.create({
      nzTitle: vacancyId ? 'Edit Vacancy' : 'Create Vacancy',
      nzContent: VacancyFormComponent,
      nzComponentParams: {
        vacancyId: vacancyId,
      },
      nzFooter: null,
      nzWidth: '80%',
    } as any);

    modal.afterClose.subscribe((result) => {
      if (result === 'created' || result === 'updated') {
        if (this.myOrganizations.length > 0) {
          this.loadVacancies(this.myOrganizations[0].id);
        }
      }
    });
  }

  openAssignTestModal(vacancy: Vacancy): void {
    const modal = this.modalService.create({
      nzTitle: `Assign Test to ${vacancy.title}`,
      nzContent: AssignTestModalComponent,
      nzComponentParams: {
        vacancy: vacancy,
      },
      nzFooter: null,
      nzWidth: '80%',
    } as any);

    modal.afterClose.subscribe((result) => {
      if (result === 'assigned') {
        if (this.myOrganizations.length > 0) {
          this.loadVacancies(this.myOrganizations[0].id);
        }
      }
    });
  }

  deleteVacancy(vacancyId: number): void {
    this.vacancyService.deleteVacancy(vacancyId).subscribe(() => {
      if (this.myOrganizations.length > 0) {
        this.loadVacancies(this.myOrganizations[0].id);
      }
    });
  }
}
