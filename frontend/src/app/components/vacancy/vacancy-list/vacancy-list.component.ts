import { Component, OnInit } from '@angular/core';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { VacancyList } from '../../../models/vacancy/vacancy';
import { VacancyFilter } from '../../../models/vacancy/vacancy-filter';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { VacancyFormComponent } from '../vacancy-form/vacancy-form.component';
import { CommonModule, DatePipe } from '@angular/common';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { RouterLink } from '@angular/router';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzTagModule } from 'ng-zorro-antd/tag';

@Component({
  selector: 'app-vacancy-list',
  templateUrl: './vacancy-list.component.html',
  styleUrls: ['./vacancy-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    NzTableModule,
    NzDividerModule,
    NzModalModule,
    NzButtonModule,
    VacancyFormComponent,
    DatePipe,
    NzPopconfirmModule,
    NzTagModule
  ]
})
export class VacancyListComponent implements OnInit {
  vacancies: VacancyList[] = [];
  loading = true;
  total = 0;
  pageSize = 10;
  pageIndex = 1;
  filter: VacancyFilter = {
    page: this.pageIndex,
    pageSize: this.pageSize,
    sortBy: 'PublishedAt',
    sortOrder: 'desc'
  };

  constructor(
    private vacancyService: VacancyService,
    private modalService: NzModalService,
    private notification: NzNotificationService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(reset = false): void {
    if (reset) {
      this.pageIndex = 1;
    }
    this.loading = true;
    this.filter.page = this.pageIndex;
    this.filter.pageSize = this.pageSize;

    this.vacancyService.getVacancies(this.filter).subscribe(data => {
      this.loading = false;
      this.vacancies = data.items;
      this.total = data.totalCount;
    },
    error => {
      this.loading = false;
      this.notification.error('Error', error.error.message || 'Failed to load vacancies');
    });
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex, sort } = params;
    const currentSort = sort.find(item => item.value !== null);
    const sortField = (currentSort && currentSort.key) || 'PublishedAt';
    const sortOrder = (currentSort && currentSort.value) || 'desc';

    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.filter.sortBy = sortField;
    this.filter.sortOrder = sortOrder === 'ascend' ? 'asc' : 'desc';
    this.loadData();
  }

  openCreateModal(): void {
    const modal = this.modalService.create({
      nzTitle: 'Create Vacancy',
      nzContent: VacancyFormComponent,
      nzWidth: '800px',
      nzFooter: null
    });

    modal.afterClose.subscribe(result => {
      if (result === 'created') {
        this.notification.success('Success', 'Vacancy created successfully.');
        this.loadData(true);
      }
    });
  }

  openEditModal(vacancy: VacancyList): void {
    const modal = this.modalService.create({
      nzTitle: 'Edit Vacancy',
      nzContent: VacancyFormComponent,
      nzWidth: '800px',
      nzData: {        vacancyId: vacancy.id
      },
      nzFooter: null
    });

    modal.afterClose.subscribe(result => {
      if (result === 'updated') {
        this.notification.success('Success', 'Vacancy updated successfully.');
        this.loadData();
      }
    });
  }

  deleteVacancy(id: number): void {
    this.vacancyService.deleteVacancy(id).subscribe(() => {
      this.notification.success('Success', 'Vacancy deleted successfully.');
      this.loadData();
    },
    error => {
      this.notification.error('Error', error.error.message || 'Failed to delete vacancy.');
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Open':
        return 'green';
      case 'Closed':
        return 'red';
      case 'Draft':
        return 'default';
      case 'Filled':
        return 'blue';
      default:
        return 'default';
    }
  }
}
